using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;
using AirandWebAPI.Persistence.Repositories;

namespace AirandWebAPI.Persistence.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(DataContext context) 
            : base(context)
        {
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}