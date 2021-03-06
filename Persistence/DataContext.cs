using AirandWebAPI.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace AirandWebAPI.Persistence
{
    public class DataContext : DbContext
    {
        
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Seed();
        }

        public virtual DbSet<User> Users { get; set; }
    }
}