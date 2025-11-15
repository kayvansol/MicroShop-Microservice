namespace MicroShop.OrderApi.Rest.Data
{
    public interface IOrderEventStoreService
    {
        Task AppendAsync(OrderEvent orderEvent);
        Task<IEnumerable<OrderEvent>> GetEventsAsync(int orderId);
    }
}
