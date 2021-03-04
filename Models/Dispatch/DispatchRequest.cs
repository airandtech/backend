using System.Collections.Generic;

namespace AirandWebAPI.Models.Dispatch
{
    public class RideOrderRequest
    {
        public RequestInfoDetails PickUp { get; set; }
        public IList<RequestInfoDetails> Delivery { get; set; }
        public int CompanyId { get; set; }
    }

    public abstract class RequestDetails
    {
        public string Address { get; set; }
        // public string RegionCode { get; set; }
        // public string AreaCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Cost { get; set; }
    }

    public class RequestInfoDetails : RequestDetails
    {

    }
}