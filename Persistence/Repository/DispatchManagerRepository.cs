using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;
using AirandWebAPI.Persistence.Repositories;

namespace AirandWebAPI.Persistence.Repositories
{
    public class DispatchManagerRepository : Repository<DispatchManager>, IDispatchManagerRepository
    {
        public DispatchManagerRepository(DataContext context) 
            : base(context)
        {
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}