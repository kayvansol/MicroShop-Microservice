
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

        public Task<bool> UpdateStatusAsync(int OrderId)
        {

            var order = _context.Orders.Where(x => x.OrderId == OrderId).SingleOrDefault();

            order.OrderStatus = (byte) EnumOrderState.Paid; // پرداخت شده

            _context.Entry(order).Property(i => i.OrderStatus).IsModified = true;

            //_context.Entry(order).State = EntityState.Modified;

            _context.SaveChanges();

            return Task.FromResult(true);

        }
    }
}
