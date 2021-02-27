using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Models.Dispatch
{
    public class DispatchResponse
    {
        public DispatchResponse() { }
        public string customerName { get; set; }
        public decimal amount { get; set; }
        public string paymentLink { get; set; }
        public string transactionId { get; set; }
        public List<int> orders { get; set; }

        public DispatchResponse(string customerName, decimal amount, string paymentLink, string transactionId, List<int> orders)
        {
            this.customerName = customerName;
            this.amount = amount;
            this.paymentLink = paymentLink;
            this.transactionId = transactionId;
            this.orders = orders;
        }
    }

}