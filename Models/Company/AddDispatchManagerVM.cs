using System.Collections.Generic;

namespace AirandWebAPI.Models.Company
{
    public class AddDispatchManagerVM
    {
        public IEnumerable<ManagerDetails> managerDetails {get;set;}
    }

    public class ManagerDetails {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

    }
}