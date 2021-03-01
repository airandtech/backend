using AirandWebAPI.Services.Contract;
using AirandWebAPI.Core;
using System;
using Microsoft.Extensions.Options;
using AirandWebAPI.Helpers;
using System.Linq;
using System.Threading.Tasks;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models;

namespace AirandWebAPI.Services.Concrete
{

    public class LocationService : ILocationService
    {
        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;

        public LocationService(IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        public async Task<bool> UpdateDriverLocation(Coordinates model, int userId)
        {
            var rider = _unitOfWork.Riders.Find(x => x.UserId.Equals(userId)).FirstOrDefault();
            if (rider != null)
            {
                rider.Latitude = model.latitude;
                rider.Longitude = model.longitude;
                rider.LastModified = DateTime.UtcNow.AddHours(1);
                await _unitOfWork.Complete();
                return true;
            }
            return false;
        }

        public GenericResponse<OrderTrackingResponse> TrackOrder(string orderId)
        {
            GenericResponse<OrderTrackingResponse> response =
                new GenericResponse<OrderTrackingResponse>(false, ResponseMessage.ERROR_OCCURRED, new OrderTrackingResponse());
            var order = _unitOfWork.Orders.Get(int.Parse(orderId));

            if (order == null)
            {
                response.message = "Invalid Order Id";
                return response;
            }

            var rider = _unitOfWork.Riders.SingleOrDefault(x => x.UserId.Equals(int.Parse(order.RiderId)));

            if (rider == null)
            {
                response.message = "Order hasn't been assigned to a rider";
                return response;
            }
            // var pickUpAndDelivery =
            //     _unitOfWork.DispatchInfo.Find(x => new int[2] { order.PickUpAddressId, order.DeliveryAddressId }.Contains(x.Id));

            var deliveryInfo = _unitOfWork.DispatchInfo.Find(x => x.Id.Equals(order.DeliveryAddressId)).FirstOrDefault();  

            var delivery = _unitOfWork.Regions
                .SingleOrDefault(x => x.AreaCode.Equals(deliveryInfo.AreaCode));

            order.PickUp = _unitOfWork.DispatchInfo.Find(x => x.Id.Equals(order.PickUpAddressId)).FirstOrDefault();
            order.Delivery = deliveryInfo;

            var user = _unitOfWork.Users.Get(rider.UserId);
            rider.User = user;

            OrderTrackingResponse orderTracking = new OrderTrackingResponse(order, delivery, rider);

            return new GenericResponse<OrderTrackingResponse>(true, ResponseMessage.SUCCESSFUL, orderTracking);

        }
    }
}