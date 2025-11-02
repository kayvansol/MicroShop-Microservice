using MicroShop.Domain.DTOs.Customer;

namespace MicroShop.Infra.Sql.Repositories.CustomerRepo
{
    public class CustomerRepository: Repository<Customer, int>, ICustomerRepository
    {

        private readonly MicroShopContext _context;
        private readonly IRepository<Customer, int> _repo;
        private readonly IMapper _mapper;

        public CustomerRepository(MicroShopContext context,
            IRepository<Customer, int> repo, IMapper mapper) : base(context)
        {
            _context = context;
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Customer> CreateAsync(Customer data)
        {
            _context.Add(data);
            _context.SaveChanges();

            return data;
        }

        public async Task<List<GetAllCustomerDto>> GetAllCustomersAsync()
        {
            var list = _repo.GetAll(predicate: x => x.Email != "");

            list = list.OrderByDescending(o => o.LastName);

            var result = _mapper.ProjectTo<GetAllCustomerDto>(list).ToList();

            return result;
        }
    }
}
