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
using Microsoft.WindowsAzure.ServiceRuntime;
using PazarWebApi.Core.Domain;

namespace AirandWebAPI.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private int take = 10;
        private int skip = 0;
        private IUnitOfWork _unitOfWork;
        private INotification _notification;
        private IMailer _mailer;
        private IEnumerable<Rider> riders;

        public OrderService(
            IUnitOfWork unitOfWork,
            INotification notification,
            IMailer mailer)
        {
            _unitOfWork = unitOfWork;
            _notification = notification;
            _mailer = mailer;
        }
        public Order GetById(int id)
        {
            return _unitOfWork.Orders.Get(id);
        }

        public async Task<DispatchResponse> Order(RideOrderRequest model)
        {
            try
            {
                Order order;
                decimal totalAmount = 0;
                List<Order> orders = new List<Order>();
                DispatchRequestInfo pickupDetails;
                foreach (var item in model.Delivery)
                {
                    pickupDetails = new DispatchRequestInfo(model.PickUp);
                    order = new Order();
                    var deliverDetails = new DispatchRequestInfo(item);
                    _unitOfWork.DispatchInfo.Add(pickupDetails);
                    order.PickUpAddressId = await _unitOfWork.Complete();

                    _unitOfWork.DispatchInfo.Add(deliverDetails);
                    order.DeliveryAddressId = await _unitOfWork.Complete();

                    order.Cost = PriceCalculator.Process(model.PickUp.AreaCode, item.AreaCode); ///refactor
                    totalAmount += order.Cost;
                    order.Status = OrderStatus.Pending;
                    order.DateCreated = DateTime.UtcNow + TimeSpan.FromHours(1);
                    order.RequestorIdentifier = pickupDetails.Email;
                    _unitOfWork.Orders.Add(order);
                    await _unitOfWork.Complete();
                    orders.Add(order);
                }
                await processDispatch(model);
                //.ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                return new DispatchResponse(model.PickUp.Name, totalAmount, this.getPaymentLink(orders));
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                return null;
            }
        }

        public async Task<bool> Accept(string requestorEmail, int riderId)
        {
            var orders = _unitOfWork.Orders.Find(x => x.RequestorIdentifier.Equals(requestorEmail) && x.Status.Equals(OrderStatus.Pending)).ToList();
            decimal amount = 0;
            string orderIds = "";

            if (orders != null && orders.Count() > 0)
            {
                foreach (var item in orders)
                {
                    var pickupDetails = _unitOfWork.DispatchInfo.Get(item.PickUpAddressId);
                    var deliveryDetails = _unitOfWork.DispatchInfo.Get(item.DeliveryAddressId);

                    var pickUpRegion = _unitOfWork.Regions.Find(x => x.AreaCode.Equals(pickupDetails.AreaCode)).FirstOrDefault();
                    var deliveryRegion = _unitOfWork.Regions.Find(x => x.AreaCode.Equals(deliveryDetails.AreaCode)).FirstOrDefault();

                    var pickUpCoord = new Coordinates(pickUpRegion.Latitude, pickUpRegion.Longitude);
                    var deliveryCoord = new Coordinates(deliveryRegion.Latitude, deliveryRegion.Longitude);

                    DirectionResponse response = await DistanceCalculator.Process(pickUpCoord, deliveryCoord);
                    var distanceAndDuration = response.routes[0].legs[0];
                    var order = _unitOfWork.Orders.Get(item.Id);
                    order.RiderId = riderId.ToString();

                    //get distance and duration and save
                    order.Distance = distanceAndDuration.distance.text;
                    order.Duration = distanceAndDuration.duration.text;
                    order.Status = OrderStatus.Completed;

                    amount += order.Cost;

                    int id = await _unitOfWork.Complete();
                    orderIds += $"{id},";
                }

                //create invoice 
                Invoice invoice = new Invoice(amount, requestorEmail, OrderStatus.Pending);
                if (orders.Count == 1) invoice.OrderId = int.Parse(orderIds.Remove(orderIds.Length - 1, 1));
                else invoice.OrderIds = orderIds;

                _unitOfWork.Invoices.Add(invoice);
                await _unitOfWork.Complete();

                //send email to customer that order has been picked up
                await sendMailToCustomer(requestorEmail, orders);
                return true;
            }
            return false;
        }

        public async Task<bool> ReceivePayment(FluttterwaveResponse response)
        {
            var invoice = _unitOfWork.Invoices.Find(x => x.CustomerEmail.Equals(response.data.customer.email)).FirstOrDefault();
            if (invoice != null)
            {
                invoice.TransactionId = response.data.id;
                invoice.AmountPaid = (decimal)(response.data.amount);
                invoice.Status = OrderStatus.Completed;
                invoice.ResponseBody = JsonConvert.SerializeObject(response);
                await _unitOfWork.Complete();
                return true;
            }
            return false;
        }

        public RiderOrders GetOrders(int userId)
        {
            RiderOrders riderOrders = new RiderOrders();
            // var rider = _unitOfWork.Riders.Find(x => x.UserId.Equals(userId)).FirstOrDefault();
            // if(rider != null){
            var dispatchDetails = _unitOfWork.DispatchInfo.GetAll();
            var orders = _unitOfWork.Orders.Find(x => x.RiderId == userId.ToString()).ToList();
            var orderWithDetails = getOrderWithDetails(orders, dispatchDetails);
            riderOrders.completed = orderWithDetails.Where(x => x.Status.Equals(OrderStatus.Completed)).ToList();
            riderOrders.inProgress = orderWithDetails.Where(x => x.Status.Equals(OrderStatus.InProgress)).ToList();
            riderOrders.pending = orderWithDetails.Where(x => x.Status.Equals(OrderStatus.Pending)).ToList();
            //}
            return riderOrders;
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
        private async Task sendMailToCustomer(string requestorEmail, List<Order> orders)
        {
            string paymentLink = getPaymentLink(orders);

            // IWebHostEnvironment env
            LocalResource localResource = RoleEnvironment.GetLocalResource("DownloadedTemplates");
            string[] paths = { localResource.RootPath, "EmailTemplate" };
            String pathToEmailTemplate = Path.Combine(paths);

            // var folderName = Path.Combine("Resources", "EmailTemplate");
            // var pathToEmailTemplate = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            decimal amount = 0;
            foreach (var item in orders)
            {
                amount += item.Cost;
            }

            using (StreamReader sr = new StreamReader(pathToEmailTemplate + "/DispatchOrder.html"))
            {
                var line = await sr.ReadToEndAsync();

                var formattedEmail = string.Format(line, "Dispatch Request", amount.ToString("#,###.00"), paymentLink);

                await _mailer.SendMailAsync(requestorEmail, requestorEmail, "Airand: Dispatch Request", formattedEmail);
            }
        }
        private string getPaymentLink(List<Order> orders)
        {
            if (orders.Count() > 1)
            {
                return "https://flutterwave.com/pay/abnkqrzycdkt";
            }
            var order = orders.FirstOrDefault();
            switch (order.Cost)
            {
                case 1000:
                    return "https://flutterwave.com/pay/airand1k";
                case 1500:
                    return "https://flutterwave.com/pay/airand1k5";
                case 2000:
                    return "https://flutterwave.com/pay/airand2k";
                case 2500:
                    return "https://flutterwave.com/pay/airand2k5";
                case 3000:
                    return "https://flutterwave.com/pay/airand3k";
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

        private async Task processDispatch(RideOrderRequest model)
        {
            //get customer details
            var region = _unitOfWork.Regions.Find(x => x.AreaCode.Equals(model.PickUp.AreaCode)).FirstOrDefault();

            //get distance between cutomer and riders
            (List<DriverCoordinates> driverCoords, DistanceMatrixResponse distanceMatrix) = await getDistanceFromCustomerToRiders(region);

            //get the 10 closest riders
            List<DriverDistance> top10Distances = getTopClosestRiders(driverCoords, distanceMatrix, 10);

            //send notification to riders
            await sendRequestToRiders(top10Distances, model);
        }

        private async Task sendRequestToRiders(List<DriverDistance> ridersDistance, RideOrderRequest model)
        {
            _notification.setRequestData(model);
            foreach (var item in ridersDistance)
            {
                _notification.setDriverDistance(item);
                var rider = riders.FirstOrDefault(x => x.Id == item.driverId);
                await _notification.SendAsync("Airand", "New Dispatch request", rider.User.Token);
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
                driverDistances.Add(new DriverDistance(driverCoords[i].driverId, new Distance(element.distance.text, element.distance.value), new Duration(element.duration.text, element.duration.value)));
                distances.Add(new Distance(element.distance.text, element.distance.value));
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

            return (ridersCoordinates, distanceMatrix);
        }

        private List<DriverCoordinates> getRidersCoordinates()
        {
            //Dictionary<int, Coordinates> dict = new Dictionary<int, Coordinates>();
            List<DriverCoordinates> driverCoordinates = new List<DriverCoordinates>();

            riders = _unitOfWork.Riders.GetAllRidersWithUsers();

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
    }
}