using System;
using System.Collections.Generic;
using System.Linq;
using AirandWebAPI.Core.Domain;
using AirandWebAPI.Core.Repositories;
using AirandWebAPI.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AirandWebAPI.Persistence.Repositories
{
    public class RiderRepository : Repository<Rider>, IRiderRepository
    {
        public RiderRepository(DataContext context) 
            : base(context)
        {
        }

        public IEnumerable<Rider> GetAllRidersWithUsers()
        {
            return DataContext.Riders
                .Include(c => c.User)
                .ToList();
        }
        public IEnumerable<Rider> GetAllActiveRidersWithUsers()
        {
            return DataContext.Riders
                .Where(x => x.LastModified > DateTime.UtcNow.AddHours(1).AddMinutes(15))
                .Include(c => c.User)
                .ToList();
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}