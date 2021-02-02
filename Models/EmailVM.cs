using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirandWebAPI.Models
{

    public class EmailVM
    {
        public EmailVM(string name, string to, string message, string subject)
        {
            this.name = name;
            this.to = to;
            this.message = message;
            this.subject = subject;
        }

        public EmailVM(){}

        public string name { get; set; }
        public string to { get; set; }
        public string message { get; set; }
        public string subject { get; set; }
    }

}