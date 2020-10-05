using System.Collections.Generic;
using AirandWebAPI.Core.Domain;

namespace AirandWebAPI.Core.Repositories
{
    public interface IRiderRepository : IRepository<Rider>
    {
        IEnumerable<Rider> GetAllRidersWithUsers();
    }
}