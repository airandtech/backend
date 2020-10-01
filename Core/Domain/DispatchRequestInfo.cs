using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI
{
    public class DispatchRequestInfo : RequestDetails 
    {
        public int Id { get; set; }
        public DateTime DateCreated {get;set;}

        [InverseProperty("Delivery")]
        public List<Order> DeliveryAddress { get; set; }

        [InverseProperty("PickUp")]
        public List<Order> PickUpAddress { get; set; }
    }
}