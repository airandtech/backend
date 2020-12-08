using System.Collections.Generic;
using System.Threading.Tasks;
using AirandWebAPI.Models.Direction;
using AirandWebAPI.Models.Dispatch;

namespace AirandWebAPI.Services
{


    public interface INotification
    {
        Task<bool> SendAsync(string title, string message, string mobile_token);
        void setRequestData(RideOrderRequest model, string transactionId);
        void setDriverDistance(DriverDistance data);
    }

}