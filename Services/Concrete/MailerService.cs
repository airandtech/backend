using System;
using Microsoft.Extensions.Options;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using AirandWebAPI.Models;
using AirandWebAPI.Helpers;

namespace AirandWebAPI.Services
{

    public class MailerService : IMailer
    {


        public MailerService()
        {
        }

        public async Task<MailResp> SendMailAsync(string email, string name, string subject, string HtmlMessage){
            MailjetClient client = new MailjetClient("a699f0c5d387bddf17f229a0232cccca", "3855a7449ff8b85b0878699d09a943e8")
            {
                Version = ApiVersion.V3_1,
            };

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }.Property(Send.Messages, new JArray {
                new JObject {
                 {"From", new JObject {
                  {"Email", "chukwuma@airandtech.com"},
                  {"Name", "Airand"}
                  }},
                 {"To", new JArray {
                  new JObject {
                   {"Email", email},
                   {"Name", name}
                   }
                  }},
                 {"Subject", subject},
                 {"TextPart", "Greetings from Airand Technologies!"},
                 {"HTMLPart", HtmlMessage}
                 }
                });
                MailjetResponse response = await client.PostAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
                    Console.WriteLine(response.GetData());
                    return new MailResp(true, response.GetData(), "");
                }
                else
                {
                    Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
                    Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
                    Console.WriteLine(response.GetData());
                    Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
                    return new MailResp(false, response.GetData() , response.GetErrorMessage() +" || "+ response.GetErrorInfo());
                }
        }
    }
}