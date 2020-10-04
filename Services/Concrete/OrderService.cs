using System;
using System.Threading.Tasks;
using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Utils;
using AirandWebAPI.Services.Contract;

namespace AirandWebAPI.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                    _unitOfWork.Orders.Add(order);
                    await _unitOfWork.Complete();
                }
                /** make call to 
                * get rider, 
                * match
                * send notification
                **/
                return true;
            }catch(Exception ex){
                string exMessage = ex.Message;
                return false;
            }


        }
    }
}