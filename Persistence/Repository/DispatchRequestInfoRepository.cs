using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;
using AirandWebAPI.Persistence.Repositories;

namespace AirandWebAPI.Persistence.Repositories
{
    public class DispatchRequestInfoRepository : Repository<DispatchRequestInfo>, IDispatchRequestInfoRepository
    {
        public DispatchRequestInfoRepository(DataContext context) 
            : base(context)
        {
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}