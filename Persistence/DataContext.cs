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
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Otp> Otps { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<DispatchManager> DispatchManagers { get; set; }
        public virtual DbSet<Merchant> Merchants { get; set; }
    }
}