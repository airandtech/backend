using System.Collections.Generic;

namespace AirandWebAPI.Models.Company
{
    public class CompanyWithDetailsVM
    {
        public CreateCompanyVM company { get; set; }
        public IEnumerable<ManagerDetails> managerDetails {get;set;}
        public IEnumerable<RiderDetails> ridersDetails { get; set; }
    }
}