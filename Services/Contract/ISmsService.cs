using System.Threading.Tasks;
using AirandWebAPI.Models;

namespace AirandWebAPI.Services
{


    public interface ISmsService
    {
        Task<bool> SendAsync(SmsBody model);
    }

}