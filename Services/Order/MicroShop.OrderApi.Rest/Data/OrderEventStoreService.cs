using Microsoft.EntityFrameworkCore;

namespace MicroShop.OrderApi.Rest.Data
{
    public class OrderEventStoreService : IOrderEventStoreService
    {
        private readonly OrderEventStoreDbContext _dbContext;

        public OrderEventStoreService(OrderEventStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AppendAsync(OrderEvent orderEvent)
        {
            _dbContext.OrderEvents.Add(orderEvent);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderEvent>> GetEventsAsync(int orderId)
        {
            return await _dbContext.OrderEvents
                .Where(x => x.OrderId == orderId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
