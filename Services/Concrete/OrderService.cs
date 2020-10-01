using AirandWebAPI.Core;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Services.Contract;

namespace AirandWebAPI.Services.Concrete
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork){
            _unitOfWork = unitOfWork;
        }
        public Order GetById(int id)
        {
            return _unitOfWork.Orders.Get(id);
        }
    }
}