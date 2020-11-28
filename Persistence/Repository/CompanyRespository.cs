using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;

namespace AirandWebAPI.Persistence.Repositories
{
    public class CompanyRespository : Repository<Company>, ICompanyRepository
    {
        public CompanyRespository(DataContext context) 
            : base(context)
        {
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}