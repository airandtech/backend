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
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<DispatchRequestInfo> DispatchRequestInfos { get; set; }
        public virtual DbSet<Rider> Riders { get; set; }
    }
}