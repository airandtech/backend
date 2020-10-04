using Newtonsoft.Json.Linq;

namespace AirandWebAPI.Models
{
    public class MailResp
    {
        public bool status {get;set;}
        public JArray respMsg {get;set;}
        public string errorMsg {get;set;}

        public MailResp(bool status, JArray respMsg, string errorMsg){
            this.status = status;
            this.respMsg = respMsg;
            this.errorMsg = errorMsg;
        }
    }
}