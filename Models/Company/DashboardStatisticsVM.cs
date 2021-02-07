using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Models.Company
{
    public class DashboardStatisticsVM
    {
        public int totalTransactionsVolume {get;set;}
        public decimal totalTransactionsValue {get;set;}
        public int todayTransactionsVolume {get;set;}
        public decimal todayTransactionsValue {get;set;}
        public int totalSuccessfulVolume {get;set;}
        public decimal totalSuccessValue {get;set;}
        public IEnumerable<Order> todayTransactions {get;set;} = new List<Order>();
        public IEnumerable<Order> totalTransactions {get;set;} = new List<Order>();
        public IEnumerable<Rider> riders {get;set;}
        public IEnumerable<Order> orders {get;set;}
    }
}