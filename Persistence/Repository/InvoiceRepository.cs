using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;

namespace AirandWebAPI.Persistence.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(DataContext context) 
            : base(context)
        {
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}