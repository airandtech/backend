using System.Collections.Generic;

namespace AirandWebAPI.Models.Dispatch
{
    public class DispatchResponse
    {
        public  DispatchResponse(){}  
        public string customerName { get; set; }
        public decimal amount { get; set; }
        public string paymentLink { get; set; }

        public  DispatchResponse(string customerName, decimal amount, string paymentLink){
            this.customerName = customerName;
            this.amount = amount;
            this.paymentLink = paymentLink;
        }  
    } 
    
}