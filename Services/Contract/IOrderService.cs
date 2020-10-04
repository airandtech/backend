using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Auth;

namespace AirandWebAPI.Services.Contract{


    public interface IOrderService
    {
        Order GetById(int id);
        Task<bool> Order(RideOrderRequest model);
        Task test();
    }

}