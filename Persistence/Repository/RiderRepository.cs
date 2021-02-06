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
            //DateTime next15mins = DateTime.UtcNow.AddHours(1).AddMinutes(15);
            return DataContext.Riders
                .Where(x => x.LastModified.AddMinutes(15) > DateTime.Now.AddHours(1))
                .Include(c => c.User)
                .ToList();
        }
        public DataContext DataContext
        {
            get { return Context as DataContext; }
        }
    }
}