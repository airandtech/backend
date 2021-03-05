using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Response;
using AirandWebAPI.Models.Auth;
using AirandWebAPI.Models.Dispatch;

namespace AirandWebAPI.Services.Contract
{


    public interface IOrderService
    {
        Order GetById(int id);
        Task<DispatchResponse> Order(RideOrderRequest model);
        Task<DispatchResponse> CompanyOrder(RideOrderRequest model);
        Task<bool> Accept(string transactionId, string requestorEmail, int riderId);
        Task<bool> ReceivePayment(FluttterwaveResponse response);
        Task test();
        RiderOrders GetOrders(int userId);
        Task<bool> ChangeStatus(ChangeStatusVM model);
        Task<bool> AssignOrderToRider(string orderId, string riderId);
        UserOrdersVM GetOrder(string transactionId);
        List<Order> GetOrdersForCompany(int limit, int offset, int userId);
    }

}