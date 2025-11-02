using Microsoft.Extensions.DependencyInjection;
using MicroShop.Infra.Sql.Repositories;
using MicroShop.Infra.Sql.Repositories.CategoryRepo;
using MicroShop.Infra.Sql.Repositories.CustomerRepo;
using MicroShop.Infra.Sql.Repositories.OrderRepo;
using MicroShop.Infra.Sql.Repositories.ProductRepo;

namespace MicroShop.Infra.Sql.Extensions
{
    public static class RegisterInfraServices
    {
        public static void AddInfraServicesRegister(this IServiceCollection service)
        {
            service.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));

            service.AddScoped<ICategoryRepository, CategoryRepository>();
            service.AddScoped<IProductRepository, ProductRepository>();
            service.AddScoped<ICustomerRepository, CustomerRepository>();
            service.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
