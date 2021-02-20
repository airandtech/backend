using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirandWebAPI.Models
{

    public class ResolveAccountRequest
    {
        public ResolveAccountRequest(string AccountNumber, string BankCode)
        {
            this.AccountNumber = AccountNumber;
            this.BankCode = BankCode;
        }

        public ResolveAccountRequest(){}

        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }
        [JsonProperty("account_bank")]
        public string BankCode { get; set; }
    }

}