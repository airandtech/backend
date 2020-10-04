using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Utils;
using AirandWebAPI.Services.Contract;
using AirandWebAPI.Models.Direction;
using System.Linq;

namespace AirandWebAPI.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private int take = 10;
        private int skip = 0;
        private IUnitOfWork _unitOfWork;
        private INotification _notification;
        private IEnumerable<Rider> riders;

        public OrderService(IUnitOfWork unitOfWork, INotification notification)
        {
            _unitOfWork = unitOfWork;
            _notification = notification;
        }
        public Order GetById(int id)
        {
            return _unitOfWork.Orders.Get(id);
        }

        public async Task<bool> Order(RideOrderRequest model)
        {
            try
            {

                Order order;
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

                    order.Cost = PriceCalculator.Process(model.PickUp.RegionCode, item.RegionCode); ///refactor
                    order.Status = "01";
                    order.DateCreated = DateTime.UtcNow + TimeSpan.FromHours(1);
                    //order.RequestorIdentifier = pickupDetails.Email;
                    _unitOfWork.Orders.Add(order);
                    await _unitOfWork.Complete();
                }
                processDispatch(model.PickUp.Email, model.PickUp.RegionCode);
                //.ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
                return true;
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                return false;
            }


        }

        public async Task test(){
            await processDispatch("musa@mail.com", "ISL001");
        }

        private async Task processDispatch(string requestorEmail, string regionCode)
        {
            //get customer details
            var region = _unitOfWork.Regions.Find(x => x.Code.Equals(regionCode)).FirstOrDefault();

            //get distance between cutomer and riders
            DistanceMatrixResponse distanceMatrix = await getDistanceFromCustomerToRider(region);

            //get the 10 closest riders
            List<Distance> top10Distances = getTopClosestRiders(distanceMatrix, 10);

            //send notification to riders
            sendRequestToRiders(riders);
        }

        private void sendRequestToRiders(IEnumerable<Rider> riders)
        {
            foreach (var item in riders)
            {
                _notification.SendAsync("Airand", "New Dispatch request", item.User.Token);
            }
        }

        private List<Distance> getTopClosestRiders(DistanceMatrixResponse distanceMatrix, int size)
        {
            List<Distance> distances = new List<Distance>();
            var rows = distanceMatrix.rows;
            foreach (var item in rows)
            {
                var element = item.elements[0];
                distances.Add(new Distance(element.distance.text, element.distance.value));
            }
            var topDistances = distances.OrderByDescending(x => x.value).Take(size).ToList();
            return topDistances;
        }

        private async Task<DistanceMatrixResponse> getDistanceFromCustomerToRider(Region region)
        {
            var driversCoordinates = getRidersCoordinates();

            DistanceMatrixResponse distanceMatrix =
                await DistanceCalculator.Process(driversCoordinates, region.Latitude, region.Longitude);

            return distanceMatrix;
        }

        private Dictionary<int, Coordinates> getRidersCoordinates()
        {
            Dictionary<int, Coordinates> dict = new Dictionary<int, Coordinates>();

            riders = _unitOfWork.Riders.GetAllRidersWithUsers();

            foreach (var item in riders)
            {
                dict.Add(item.Id, new Coordinates(item.Latitude, item.Longitude));
            }
            return dict;
        }

        private IEnumerable<Rider> nextRiders(IEnumerable<Rider> riders)
        {
            IEnumerable<Rider> nextRiders;
            nextRiders = riders.Skip(skip).Take(take);
            skip += 10;
            return nextRiders;
        }
    }
}