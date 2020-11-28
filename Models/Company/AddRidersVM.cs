using System.Collections.Generic;
namespace AirandWebAPI.Models.Company
{
    public class AddRidersVM
    {
        public IEnumerable<RiderDetails> ridersDetails { get; set; }
    }

    public class RiderDetails{
        public string Name { get; set; }
        public string Phone { get; set; }

    }
}