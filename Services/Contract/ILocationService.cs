using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.Direction;

namespace AirandWebAPI.Services.Contract
{


    public interface ILocationService
    {
        Task<bool> UpdateDriverLocation(Coordinates model, int userId);
    }

}