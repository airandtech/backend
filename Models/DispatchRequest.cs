using System.Collections.Generic;

namespace AirandWebAPI
{
    public class RideOrderRequest
    {
        public RequestInfoDetails PickUp {get;set;}
        public IList<RequestInfoDetails> Delivery {get;set;}
    }   

    public abstract class RequestDetails{
        public string Address {get;set;}
        public string RegionCode {get;set;}
        public string Name {get;set;}
        public string Email {get;set;}
        public string Phone {get;set;}
    } 

    public class RequestInfoDetails : RequestDetails{

    }
}