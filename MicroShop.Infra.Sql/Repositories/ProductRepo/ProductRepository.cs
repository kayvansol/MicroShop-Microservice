using MicroShop.Domain.DTOs.Product;

namespace MicroShop.Infra.Sql.Repositories.ProductRepo
{
    public class ProductRepository : Repository<Product, int>, IProductRepository
    {
        private readonly MicroShopContext _context;
        private readonly IRepository<Product, int> _repo;
        private readonly IMapper _mapper;

        public ProductRepository(MicroShopContext context,
            IRepository<Product, int> repo, IMapper mapper) : base(context)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Product> CreateAsync(Product data)
        {
            _context.Add(data);
            _context.SaveChanges();

            return data;
        }

        public async Task<List<GetAllProductDto>> GetAllProductsAsync()
        {
            var list = _repo.GetAll(predicate: x => x.Price != 0, Includes: o => o.Include(c => c.Category));
            
            list = list.OrderByDescending(o => o.ProductName);

            var result = _mapper.ProjectTo<GetAllProductDto>(list).ToList();

            return result;
        }
    }
}
