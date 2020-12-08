using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AirandWebAPI.Core.Domain
{
    public class Rider : BaseEntity
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string DeliveriesCompleted { get; set; }
        public string Status {get;set;}
        public int CreatedBy {get;set;}

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
