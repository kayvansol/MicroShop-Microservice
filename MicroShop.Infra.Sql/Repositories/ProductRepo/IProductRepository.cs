using MicroShop.Domain.DTOs.Product;

namespace MicroShop.Infra.Sql.Repositories.ProductRepo
{
    public interface IProductRepository : IRepository<Product, int>
    {
        Task<List<GetAllProductDto>> GetAllProductsAsync();

        Task<Product> CreateAsync(Product data);
    }
}
