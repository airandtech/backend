using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AirandWebAPI.Core.Domain
{
    public class Order : BaseEntity
    {
        public int DeliveryAddressId { get; set; }
        public DispatchRequestInfo Delivery { get; set; }

        public int PickUpAddressId { get; set; }
        public DispatchRequestInfo PickUp { get; set; }
        
        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost {get;set;}
        public string Distance {get;set;}
        public string Duration {get;set;}
        public string RiderId {get;set;}
        public string RequestorIdentifier {get;set;}
        public string Status {get;set;}
    }
}
