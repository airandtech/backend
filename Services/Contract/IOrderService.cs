using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models;
using AirandWebAPI.Models.Auth;

namespace AirandWebAPI.Services.Contract{


    public interface IOrderService
    {
        Order GetById(int id);
        Task<bool> Order(RideOrderRequest model);
        Task<bool> Accept(string requestorEmail, int riderId);
        Task<bool> ReceivePayment(FluttterwaveResponse response);
        Task test();
    }

}