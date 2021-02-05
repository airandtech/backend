using System;
using System.Collections.Generic;

namespace AirandWebAPI.Core.Domain
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime DateCreated {get;set;}
        public DateTime LastModified {get;set;} = DateTime.UtcNow.AddHours(1);
    }
}
