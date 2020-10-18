using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirandWebAPI.Models
{

    public class SmsBody
    {
        public SmsBody(string from, string to, string message)
        {
            this.from = from;
            this.to = to;
            this.message = message;
        }

        public SmsBody(){}

        [JsonProperty("from")]
        public string from { get; set; }
        public string to { get; set; }
        public string message { get; set; }
    }

}