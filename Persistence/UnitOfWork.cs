using System.Threading.Tasks;
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
            Riders = new RiderRepository(_context);
            DispatchInfo = new DispatchRequestInfoRepository(_context);
            Regions = new RegionRepository(_context);
            Invoices = new InvoiceRepository(_context);
            Otps = new OtpRepository(_context);
        }

        public IUserRepository Users { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IRiderRepository Riders { get; private set; }
        public IDispatchRequestInfoRepository DispatchInfo { get; private set; }
        public IRegionRepository Regions { get; private set; }
        public IInvoiceRepository Invoices { get; private set; }
        public IOtpRepository Otps { get; private set; }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}