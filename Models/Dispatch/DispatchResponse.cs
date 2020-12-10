using System.Collections.Generic;

namespace AirandWebAPI.Models.Dispatch
{
    public class DispatchResponse
    {
        public  DispatchResponse(){}  
        public string customerName { get; set; }
        public decimal amount { get; set; }
        public string paymentLink { get; set; }
        public string transactionId {get;set;}

        public  DispatchResponse(string customerName, decimal amount, string paymentLink, string transactionId){
            this.customerName = customerName;
            this.amount = amount;
            this.paymentLink = paymentLink;
            this.transactionId = transactionId;
        }  
    } 
    
}