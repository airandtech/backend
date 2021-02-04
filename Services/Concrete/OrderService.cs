using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Utils;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Models.Direction;
using System.Linq;
using System.IO;
using AirandWebAPI.Models.Response;
using Newtonsoft.Json;
using AirandWebAPI.Models.Dispatch;
using PazarWebApi.Core.Domain;
using AirandWebAPI.Models;
using AutoMapper;
using AirandWebAPI.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace AirandWebAPI.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private int take = 10;
        private int skip = 0;
        private readonly string notificationBaseUrl = "https://partners.airand.net/";
        private IUnitOfWork _unitOfWork;
        private INotification _notification;
        private IMailer _mailer;
        private IEnumerable<Rider> riders;
        private ISmsService _smsService;
        private IMapper _mapper;
        private readonly ILogger _logger;

        public OrderService(
            IUnitOfWork unitOfWork,
            INotification notification,
            IMailer mailer,
            ISmsService smsService,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _notification = notification;
            _mailer = mailer;
            _smsService = smsService;
            _mapper = mapper;
            _logger = logger;
        }
        public Order GetById(int id)
        {
            return _unitOfWork.Orders.Get(id);
        }

        public async Task<DispatchResponse> Order(RideOrderRequest model)
        {

            Order order;
            decimal totalAmount = 0;
            List<Order> orders = new List<Order>();
            DispatchRequestInfo pickupDetails;
            string transactionId = Guid.NewGuid().ToString();


            pickupDetails = new DispatchRequestInfo(model.PickUp);
            _unitOfWork.DispatchInfo.Add(pickupDetails);
            await _unitOfWork.Complete();
            foreach (var item in model.Delivery)
            {

                order = new Order();
                var deliverDetails = new DispatchRequestInfo(item);

                order.PickUpAddressId = pickupDetails.Id;

                _unitOfWork.DispatchInfo.Add(deliverDetails);
                await _unitOfWork.Complete();
                order.DeliveryAddressId = deliverDetails.Id;

                order.Cost = PriceCalculator.Process(model.PickUp.RegionCode, item.RegionCode); ///refactor
                totalAmount += order.Cost;
                order.Status = OrderStatus.Created;
                order.DateCreated = DateTime.UtcNow + TimeSpan.FromHours(1);
                order.RequestorIdentifier = pickupDetails.Email;
                order.TransactionId = transactionId;
                order.PaymentStatus = 0;
                _unitOfWork.Orders.Add(order);
                await _unitOfWork.Complete();
                orders.Add(order);

                //await Task.WhenAll(dispatchDetailsTask, dispatchInfoTask, ordersTask);
            }
            //var managerTask = sendToManager(model, transactionId);
            var dispatchTask = processDispatch(model, transactionId);

            await Task.WhenAll(dispatchTask);
            return new DispatchResponse(model.PickUp.Name, totalAmount, this.getPaymentLink(orders), transactionId);
        }

        public async Task<bool> Accept(string transactionId, string requestorEmail, int riderId)
        {
            var orders = _unitOfWork.Orders.Find(
                x => x.TransactionId.Equals(transactionId)
                && x.Status.Equals(OrderStatus.Created)
                ).ToList();
            decimal amount = 0;
            string orderIds = "";

            if (orders != null && orders.Count() > 0)
            {
                foreach (var item in orders)
                {
                    var pickupDetails = _unitOfWork.DispatchInfo.Get(item.PickUpAddressId);
                    var deliveryDetails = _unitOfWork.DispatchInfo.Get(item.DeliveryAddressId);

                    var pickUpRegion = _unitOfWork.Regions.Find(x => x.Code.Equals(pickupDetails.RegionCode)).FirstOrDefault();
                    var deliveryRegion = _unitOfWork.Regions.Find(x => x.Code.Equals(deliveryDetails.RegionCode)).FirstOrDefault();

                    var pickUpCoord = new Coordinates(pickUpRegion.Latitude, pickUpRegion.Longitude);
                    var deliveryCoord = new Coordinates(deliveryRegion.Latitude, deliveryRegion.Longitude);

                    DirectionResponse response = await DistanceCalculator.Process(pickUpCoord, deliveryCoord);
                    var distanceAndDuration = response.routes[0].legs[0];
                    var order = _unitOfWork.Orders.Get(item.Id);
                    order.RiderId = riderId.ToString();

                    //get distance and duration and save
                    order.Distance = distanceAndDuration.distance.text;
                    order.Duration = distanceAndDuration.duration.text;
                    order.Status = OrderStatus.Pending;

                    amount += order.Cost;

                    await _unitOfWork.Complete();
                    orderIds += $"{order.Id},";
                }

                //create invoice 
                Invoice invoice = new Invoice(amount, requestorEmail, OrderStatus.Pending, transactionId);
                invoice.DateCreated = DateTime.Now;

                if (orders.Count == 1) invoice.OrderId = int.Parse(orderIds.Remove(orderIds.Length - 1, 1));
                else invoice.OrderIds = orderIds;

                _unitOfWork.Invoices.Add(invoice);
                await _unitOfWork.Complete();

                //send email to customer that order has been picked up
                await sendMailToCustomer(requestorEmail, orders, riderId);
                return true;
            }
            return false;
        }

        public async Task<bool> ReceivePayment(FluttterwaveResponse response)
        {
            var invoice = _unitOfWork.Invoices
                .Find(x => x.CustomerEmail.Equals(response.data.customer.email)
                && !x.Status.Equals(OrderStatus.Completed)
                && response.data.amount.Equals((int)x.Amount)
                )
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
            if (invoice != null)
            {
                invoice.TransactionId = response.data.id;
                invoice.AmountPaid = (decimal)(response.data.amount);
                invoice.Status = OrderStatus.Completed;
                invoice.ResponseBody = JsonConvert.SerializeObject(response);
                await _unitOfWork.Complete();

                var orders = _unitOfWork.Orders.Find(x => x.TransactionId.Equals(invoice.AirandTxnId));
                foreach (var item in orders)
                {
                    item.PaymentStatus = 1;
                }
                await _unitOfWork.Complete();
                return true;
            }
            return false;
        }

        public RiderOrders GetOrders(int userId)
        {
            RiderOrders riderOrders = new RiderOrders();
            var dispatchDetails = _unitOfWork.DispatchInfo.GetAll();
            var orders = _unitOfWork.Orders.Find(x => x.RiderId == userId.ToString()).OrderByDescending(x => x.DateCreated).ToList();
            var orderWithDetails = getOrderWithDetails(orders, dispatchDetails);
            riderOrders.completed = orderWithDetails.Where(x => x.Status.Equals(OrderStatus.Completed)).Take(50).ToList();
            riderOrders.inProgress = orderWithDetails.Where(x => x.Status.Equals(OrderStatus.InProgress)).ToList();
            riderOrders.pending = orderWithDetails.Where(x => x.Status.Equals(OrderStatus.Pending)).ToList();
            //}
            return riderOrders;
        }

        public UserOrdersVM GetOrder(string transactionId)
        {
            UserOrdersVM userOrders = new UserOrdersVM();
            var order = new Order();
            var orders = _unitOfWork.Orders.GetOrdersLocations(transactionId);
            var ordersWithLocation = new List<Order>();
            foreach (var item in orders)
            {
                order = item;
                order.Delivery = _unitOfWork.DispatchInfo.Get(item.DeliveryAddressId);
                ordersWithLocation.Add(order);
            }

            var ordersDto = _mapper.Map<List<OrderDto>>(ordersWithLocation);

            userOrders.orders = ordersDto;
            userOrders.pickupDetails = _unitOfWork.DispatchInfo.Get(orders.FirstOrDefault().PickUpAddressId);
            return userOrders;
        }

        public bool ChangeStatus(ChangeStatusVM model)
        {
            var order = _unitOfWork.Orders.Get(int.Parse(model.orderId));
            if (order != null)
            {
                order.Status = OrderStatus.GetStatusFromCode(model.status);
                _unitOfWork.Complete();
                return true;
            }
            return false;
        }
        private async Task sendMailToCustomer(string requestorEmail, List<Order> orders, int riderId)
        {
            try
            {
                string riderPhone = "N/A";
                string managerPhone = "N/A";
                string paymentLink = getPaymentLink(orders);

                // IWebHostEnvironment env
                // LocalResource localResource = RoleEnvironment.GetLocalResource("Resources");
                // string[] paths = { localResource.RootPath, "EmailTemplate" };
                // String pathToEmailTemplate = Path.Combine(paths);

                var folderName = Path.Combine("Resources", "EmailTemplate");
                var pathToEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                decimal amount = 0;
                foreach (var item in orders)
                {
                    amount += item.Cost;
                }
                var rider = _unitOfWork.Riders.Get(riderId);
                if(rider != null){
                    var riderUser = _unitOfWork.Users.Get(rider.UserId);
                    riderPhone = riderUser.Phone;
                    var company = _unitOfWork.Companies.SingleOrDefault(x => x.UserId.Equals(riderUser.CreatedBy));
                    var manager = _unitOfWork.DispatchManagers.SingleOrDefault(x => x.CompanyId.Equals(company.Id));
                    managerPhone = manager.Phone;
                } 

                

                using (StreamReader sr = new StreamReader(pathToEmailTemplate + "/DispatchOrder.html"))
                {
                    var line = await sr.ReadToEndAsync();

                    var formattedEmail = string.Format(line, "Dispatch Request", amount.ToString("#,###.00"), paymentLink, managerPhone, riderPhone);

                    await _mailer.SendMailAsync(requestorEmail, requestorEmail, "Airand: Dispatch Request", formattedEmail);
                }

                //send sms notification
                string message = $"Airand: Your order has been accepted. See payment link {paymentLink}. For Enquiries contact Rider on {riderPhone} or his manager on {managerPhone}";
                int pickupAddressId = orders.FirstOrDefault().PickUpAddressId;
                var user = _unitOfWork.DispatchInfo.Get(pickupAddressId);//refactor this 

                //check if sms is sent to the sender of the request
                SmsBody smsBody = new SmsBody("Airand", user.Phone, message);

                await _smsService.SendAsync(smsBody);
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                Debug.WriteLine($"*****{exMessage}");
                Console.WriteLine($"*****{exMessage}");
            }
        }
        private string getPaymentLink(List<Order> orders)
        {
            decimal cost = 0;
            if (orders.Count() > 1)
            {
                orders.ForEach(x => cost += x.Cost);
            }
            else
            {
                cost = orders.SingleOrDefault().Cost;
            }

            switch ((int)cost)
            {
                case 1000:
                    //return "https://ravesandbox.flutterwave.com/pay/ifieygi2gotd";
                    return "https://flutterwave.com/pay/ehaynzymuswd";
                case 1500:
                    //return "https://ravesandbox.flutterwave.com/pay/aljakozdf7j2";
                    return "https://flutterwave.com/pay/ejunjscgtymy";
                case 2000:
                    //return "https://ravesandbox.flutterwave.com/pay/y9tqicwnkw84";
                    return "https://flutterwave.com/pay/7ubgyhnbdmds";
                case 2500:
                    return "https://flutterwave.com/pay/btfzwouc7cig";
                case 3000:
                    return "https://flutterwave.com/pay/m4ibqgj62bxo";
                case 3500:
                    return "https://flutterwave.com/pay/wltb9wjo6ke5";
                case 4000:
                    return "https://flutterwave.com/pay/gjqc9tmtpsan";
                case 4500:
                    return "https://flutterwave.com/pay/moknanghqoxt";
                case 5000:
                    return "https://flutterwave.com/pay/6m0spzszgm9j";
                case 5500:
                    return "https://flutterwave.com/pay/xlfapgdx0qps";
                case 6000:
                    return "https://flutterwave.com/pay/adb15t1ikfd1";
                case 6500:
                    return "https://flutterwave.com/pay/fynayozn03td";
                case 7000:
                    return "https://flutterwave.com/pay/fuhmk5g8ykg7";
                case 7500:
                    return "https://flutterwave.com/pay/gheu1xv7bq9k";
                case 8000:
                    return "https://flutterwave.com/pay/pe8omfu6s6f4";
                case 8500:
                    return "https://flutterwave.com/pay/kptxg4nz3kmb";
                case 9000:
                    return "https://flutterwave.com/pay/dtobnvifhnq7";
                case 9500:
                    return "https://flutterwave.com/pay/oxusqegcqtf9";
                case 10000:
                    return "https://flutterwave.com/pay/9ylwy4fccrfw";
                default:
                    return "https://flutterwave.com/pay/abnkqrzycdkt";
            }
        }
        public async Task test()
        {
            //string email = "timolor94@gmail.com";
            //await processDispatch("musa@mail.com", "ISL001");
            //var orders = _unitOfWork.Orders.Find(x => x.RequestorIdentifier.Equals(email) && x.Status.Equals("01")).ToList();
            //await sendMailToCustomer(email, orders);
        }

        private async Task processDispatch(RideOrderRequest model, string transactionId)
        {
            //get customer details
            var region = _unitOfWork.Regions.Find(x => x.AreaCode.Equals(model.PickUp.AreaCode)).FirstOrDefault();

            //get distance between cutomer and riders
            //(List<DriverCoordinates> driverCoords, DistanceMatrixResponse distanceMatrix) = await getDistanceFromCustomerToRiders(region);

            //get all the riders
            var ridersCoordinates = getRidersCoordinates();
            var chunkedRiders = ridersCoordinates.ChunkBy(10);
            foreach (var item in chunkedRiders)
            {
                DistanceMatrixResponse distanceMatrix =
               await DistanceCalculator.Process(item, region.Latitude, region.Longitude);
                List<DriverDistance> top10Distances = getTopClosestRiders(item, distanceMatrix, 10);
                if (top10Distances != null)
                {
                    await sendRequestToRidersAndManagers(top10Distances, model, transactionId);
                    //return;
                }
            }

            //get the 10 closest riders
            // List<DriverDistance> top10Distances = getTopClosestRiders(driverCoords, distanceMatrix, 10);
            // if(top10Distances != null){
            //     await sendRequestToRidersAndManagers(top10Distances, model, transactionId);
            //     return;
            // }
            _logger.LogInformation("++++ No Riders found");

            //send notification to riders

        }

        private async Task sendRequestToRidersAndManagers(List<DriverDistance> ridersDistance, RideOrderRequest model, string transactionId)
        {
            _notification.setRequestData(model, transactionId);
            foreach (var item in ridersDistance)
            {
                _notification.setDriverDistance(item);
                var rider = riders.FirstOrDefault(x => x.Id == item.driverId);
                await _notification.SendAsync("Airand", "New Dispatch request", rider.User.Token);

                //send request to the rider's manager
                var managerTask = sendToManagerSpecificToRider(model, transactionId, rider);
            }
        }

        private List<DriverDistance> getTopClosestRiders(List<DriverCoordinates> driverCoords, DistanceMatrixResponse distanceMatrix, int size)
        {
            List<Distance> distances = new List<Distance>();
            List<DriverDistance> driverDistances = new List<DriverDistance>();
            var rows = distanceMatrix.rows;
            int i = 0;
            foreach (var item in rows)
            {
                var element = item.elements[0];
                if (element.distance != null && element.duration != null)
                {
                    driverDistances.Add(new DriverDistance(driverCoords[i].driverId, new Distance(element.distance.text, element.distance.value), new Duration(element.duration.text, element.duration.value)));
                    distances.Add(new Distance(element.distance.text, element.distance.value));
                }
                i++;
            }
            var topDriverDistances = driverDistances.OrderByDescending(x => x.distance.value).Take(size).ToList();
            //var topDistances = distances.OrderByDescending(x => x.value).Take(size).ToList();
            return topDriverDistances;
        }

        private async Task<(List<DriverCoordinates>, DistanceMatrixResponse)> getDistanceFromCustomerToRiders(Region region)
        {
            var ridersCoordinates = getRidersCoordinates();

            DistanceMatrixResponse distanceMatrix =
                await DistanceCalculator.Process(ridersCoordinates, region.Latitude, region.Longitude);


            //await sendRequestToRidersAndManagers(top10Distances, model, transactionId)

            return (ridersCoordinates, distanceMatrix);
        }

        private List<DriverCoordinates> getRidersCoordinates()
        {
            //Dictionary<int, Coordinates> dict = new Dictionary<int, Coordinates>();
            List<DriverCoordinates> driverCoordinates = new List<DriverCoordinates>();

            riders = _unitOfWork.Riders.GetAllActiveRidersWithUsers();

            foreach (var item in riders)
            {
                driverCoordinates.Add(new DriverCoordinates(item.Id, new Coordinates(item.Latitude, item.Longitude)));
                //dict.Add(item.Id, new Coordinates(item.Latitude, item.Longitude));
            }
            return driverCoordinates;
            //return dict;
        }

        private IEnumerable<Rider> nextRiders(IEnumerable<Rider> riders)
        {
            IEnumerable<Rider> nextRiders;
            nextRiders = riders.Skip(skip).Take(take);
            skip += 10;
            return nextRiders;
        }

        private List<Order> getOrderWithDetails(List<Order> orders, IEnumerable<DispatchRequestInfo> details)
        {

            List<Order> ordersList = new List<Order>();
            Order order;
            foreach (var item in orders)
            {
                order = new Order();
                item.PickUp = details.FirstOrDefault(x => x.Id == item.PickUpAddressId);
                item.Delivery = details.FirstOrDefault(x => x.Id == item.DeliveryAddressId);
                order = item;
                ordersList.Add(order);
            }

            return ordersList;
        }
        //const transactionId  = req.query.transactionId
        private async Task sendToManager(RideOrderRequest model, string transactionId)
        {

            string orderLink = $"{notificationBaseUrl}/orderDetails?id={transactionId}";
            //get all managers
            var managers = _unitOfWork.DispatchManagers.GetAll();
            var folderName = Path.Combine("Resources", "EmailTemplate");
            var pathToEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            orderLink = ShrinkURL.Process(orderLink);


            foreach (var item in managers)
            {
                //send email notification
                using (StreamReader sr = new StreamReader(pathToEmailTemplate + "/OrderNotification.html"))
                {
                    var line = await sr.ReadToEndAsync();

                    var formattedEmail = string.Format(line, item.Name, orderLink);

                    await _mailer.SendMailAsync(item.Email, item.Name, "Airand: Dispatch Request", formattedEmail);
                }

                //send sms notification
                string message = $"Airand: New Dispatch Request click {orderLink} to view details. ";
                SmsBody smsBody = new SmsBody("Airand", item.Phone, message);

                await _smsService.SendAsync(smsBody);
            }


        }

        private async Task sendToManagerSpecificToRider(RideOrderRequest model, string transactionId, Rider rider)
        {

            string orderLink = $"{notificationBaseUrl}/orderDetails?id={transactionId}&riderId={rider.UserId}";
            //get company
            var company = _unitOfWork.Companies.Find(x => x.UserId.Equals(rider.CreatedBy)).FirstOrDefault();
            if (company == null)
                return;
            var dispatchManagers = _unitOfWork.DispatchManagers.Find(x => x.CompanyId.Equals(company.Id)).ToList();
            if (dispatchManagers == null)
                return;


            var folderName = Path.Combine("Resources", "EmailTemplate");
            var pathToEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            orderLink = ShrinkURL.Process(orderLink);

            foreach (var item in dispatchManagers)
            {
                using (StreamReader sr = new StreamReader(pathToEmailTemplate + "/OrderNotification.html"))
                {
                    var line = await sr.ReadToEndAsync();

                    var formattedEmail = string.Format(line, item.Name, orderLink, $"{rider.User.FirstName} {rider.User.LastName}", rider.User.Phone);

                    await _mailer.SendMailAsync(item.Email, item.Name, "Airand: Dispatch Request", formattedEmail);
                }

                //send sms notification
                string message = $"Airand: Dispatch Request for {rider.User.FirstName} ({rider.User.Phone}) click {orderLink} to view details. ";
                SmsBody smsBody = new SmsBody("Airand", item.Phone, message);

                await _smsService.SendAsync(smsBody);
            }

        }
    }
}