using Microsoft.EntityFrameworkCore;

namespace MicroShop.OrderApi.Rest.Data
{
    public class OrderEventStoreDbContext : DbContext
    {
        public OrderEventStoreDbContext(DbContextOptions<OrderEventStoreDbContext> options)
            : base(options)
        {
        }

        public DbSet<OrderEvent> OrderEvents => Set<OrderEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderEvent>().ToTable("OrderEvents");
            modelBuilder.Entity<OrderEvent>().HasKey(x => x.Id);
        }

    }
}
