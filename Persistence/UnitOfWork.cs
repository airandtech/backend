using AirandWebAPI.Core;
using AirandWebAPI.Core.Repositories;
using AirandWebAPI.Persistence.Repositories;

namespace AirandWebAPI.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Orders = new OrderRepository(_context);
        }

        public IUserRepository Users { get; private set; }

        public IOrderRepository Orders { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}