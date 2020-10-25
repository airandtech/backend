using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Models.Dispatch
{
    public class RiderOrders
    {
        public  RiderOrders(){}  
        public List<Order> pending { get; set; }
        public List<Order> completed { get; set; }
        public List<Order> inProgress { get; set; }

        public  RiderOrders(List<Order> pending, List<Order> completed, List<Order> inProgress){
            this.pending = pending;
            this.completed = completed;
            this.inProgress = inProgress;
        }  
    } 
    
}