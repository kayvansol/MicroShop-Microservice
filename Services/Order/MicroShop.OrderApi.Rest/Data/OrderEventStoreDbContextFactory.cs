using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MicroShop.OrderApi.Rest.Data
{
    public class OrderEventStoreDbContextFactory : IDesignTimeDbContextFactory<OrderEventStoreDbContext>
    {
        public OrderEventStoreDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<OrderEventStoreDbContext>();
            var cs = configuration.GetConnectionString("OrderEventStoreConnection")
                     ?? "Server=localhost;Database=OrderEventStoreDb;Trusted_Connection=True;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(cs);

            return new OrderEventStoreDbContext(optionsBuilder.Options);
        }
    }
}
