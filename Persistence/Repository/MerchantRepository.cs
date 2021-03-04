using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;

namespace AirandWebAPI.Persistence.Repositories
{
    public class MerchantRepository : Repository<Merchant>, IMerchantRepository
    {
        public MerchantRepository(DataContext context) 
            : base(context)
        {
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}