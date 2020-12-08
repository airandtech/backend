using System;
using AirandWebAPI.Models.Dispatch;

namespace AirandWebAPI.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal Cost {get;set;}
        public string Distance {get;set;}
        public string Duration {get;set;}
        public string RiderId {get;set;}
        public string RequestorIdentifier {get;set;}
        public string TransactionId {get;set;}
        public string Status {get;set;}
        public int DeliveryAddressId { get; set; }
        public RequestInfoDetails Delivery {get;set;}
        public DateTime DateCreated {get;set;}
        public DateTime LastModified {get;set;}
    }
}