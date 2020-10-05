using System.Threading.Tasks;

namespace AirandWebAPI.Services
{


    public interface INotification
    {
        Task<bool> SendAsync(string title, string message, string mobile_token);
    }

}