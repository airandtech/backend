using System.Collections.Generic;
using System.Linq;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;
using AirandWebAPI.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public List<Order> GetOrderWithLocations(string id)
        {
                return DataContext.Orders
                .Where(c => c.RiderId == id)
                .Include(c => c.Delivery)
                .Include(c => c.PickUp)
                .ToList();
        }

          // public IEnumerable<Course> GetCoursesWithAuthors(int pageIndex, int pageSize = 10)
        // {
        //     return PlutoContext.Courses
        //         .Include(c => c.Author)
        //         .OrderBy(c => c.Name)
        //         .Skip((pageIndex - 1) * pageSize)
        //         .Take(pageSize)
        //         .ToList();
        // }
    }
}