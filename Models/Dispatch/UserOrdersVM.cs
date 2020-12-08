using System.Collections.Generic;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Models.DTOs;

namespace AirandWebAPI.Models.Dispatch
{
    public class UserOrdersVM
    {
        public  UserOrdersVM(){}  
        public List<OrderDto> orders { get; set; }
        public RequestInfoDetails pickupDetails { get; set; } 
    } 
    
}