using System.Collections.Generic;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Auth;

namespace AirandWebAPI.Services.Contract{


    public interface IOrderService
    {
        Order GetById(int id);
    }

}