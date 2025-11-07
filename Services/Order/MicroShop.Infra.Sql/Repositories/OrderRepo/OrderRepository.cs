
using MicroShop.Domain;
using MicroShop.Domain.Enums;
using System.Data;

namespace MicroShop.Infra.Sql.Repositories.OrderRepo
{
    public class OrderRepository : Repository<Order, int>, IOrderRepository
    {

        private readonly MicroShopContext _context;
        private readonly IRepository<Order, int> _repo;
        private readonly IMapper _mapper;

        public OrderRepository(MicroShopContext context,
            IRepository<Order, int> repo, IMapper mapper) : base(context)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Order> CreateAsync(Order data)
        {
            _context.Add(data);
            _context.SaveChanges();

            return data;
        }

        public async Task<bool> UpdateStatusAsync(int OrderId, EnumOrderState State)
        {

            var order = await _context.Orders.Where(x => x.OrderId == OrderId).SingleOrDefaultAsync();

            if (order == null)
                return false;

            order.OrderStatus = (byte) State;

            _context.Entry(order).Property(i => i.OrderStatus).IsModified = true;

            //_context.Entry(order).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return true;

        }

        public async Task<bool> UpdateInventoriesAsync(int OrderId)
        {

            var orderItems = await _context.OrderItems.Where(x => x.OrderId == OrderId).ToListAsync();

            foreach (var item in orderItems)
            {
                var product = _context.Products.Where(x => x.ProductId == item.ProductId).SingleOrDefault();

                product.Inventory -= item.Quantity;

                if (product == null)
                    continue;

                _context.Entry(product).Property(i => i.Inventory).IsModified = true;

            }

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
