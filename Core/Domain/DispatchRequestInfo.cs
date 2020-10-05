using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI
{
    public class DispatchRequestInfo : RequestInfoDetails
    {
        public int Id { get; set; }
        public DateTime DateCreated {get;set;}

        [InverseProperty("Delivery")]
        public List<Order> DeliveryAddress { get; set; }

        [InverseProperty("PickUp")]
        public List<Order> PickUpAddress { get; set; }

        public DispatchRequestInfo(){}
        public DispatchRequestInfo(RequestDetails data){
            this.Address = data.Address;
            this.Email = data.Email;
            this.Name = data.Name;
            this.Phone = data.Phone;
            this.RegionCode = data.RegionCode;
            this.DateCreated = DateTime.UtcNow + TimeSpan.FromHours(1);
        }
    }
}