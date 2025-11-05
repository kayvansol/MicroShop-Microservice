
namespace MicroShop.Infra.Sql.Repositories.OrderRepo
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        //Task<List<GetAllCustomerDto>> GetAllCustomersAsync();

        Task<Order> CreateAsync(Order data);

        Task<bool> UpdateStatusAsync(int OrderId);

    }
}
