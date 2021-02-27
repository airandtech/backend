using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Core.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        List<Order> GetOrdersLocations(string id);
        Order GetOrderWithLocation(int id);
    }
}