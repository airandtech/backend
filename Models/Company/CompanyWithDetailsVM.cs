using System.Collections.Generic;
using AirandWebAPI.Models.DTOs;

namespace AirandWebAPI.Models.Company
{
    public class CompanyWithDetailsVM
    {
        public UserDto user { get; set; }
        public CreateCompanyVM company { get; set; }
        public IEnumerable<ManagerDetails> managerDetails {get;set;}
        public IEnumerable<RiderDetails> ridersDetails { get; set; }
    }
}