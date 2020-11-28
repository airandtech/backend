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
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<DispatchManager> DispatchManagers { get; set; }
    }
}
