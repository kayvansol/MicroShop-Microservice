
using MicroShop.Domain.DTOs.Order;
using MicroShop.Domain.Enums;

namespace MicroShop.Infra.Sql.Repositories.OrderRepo
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        //Task<List<GetAllCustomerDto>> GetAllCustomersAsync();

        Task<Order> CreateAsync(Order data);

        Task<bool> UpdateStatusAsync(int OrderId, EnumOrderState State);

        Task<bool> UpdateInventoriesAsync(int OrderId);
        Task<List<GetAllOrders>> GetAllOrders();
        Task<List<GetOrderItems>> GetOrderItems(int OrderId);
    }
}
