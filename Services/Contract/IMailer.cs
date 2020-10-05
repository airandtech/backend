using System.Threading.Tasks;
using AirandWebAPI.Models;

namespace AirandWebAPI.Services
{


    public interface IMailer
    {
       Task<MailResp> SendMailAsync(string email, string name, string subject, string HtmlMessage);
    }

}