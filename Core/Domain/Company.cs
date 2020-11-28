using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirandWebAPI.Core.Domain
{
    public class Company : BaseEntity
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string OfficeArea { get; set; }
    }
}
