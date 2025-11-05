
using AutoMapper;
using Payment.API.Context;
using Payment.API.Repositories;
using Payment.API.Repositories.PaymentRepo;

namespace Payment.API.Repositories.PaymentRepo
{
    public class PaymentRepository : Repository<Payment.API.Entities.Payment, int>, IPaymentRepository
    {
        private readonly MicroShopPaymentContext _context;
        private readonly IMapper _mapper;

        public PaymentRepository(MicroShopPaymentContext context,
            IRepository<Entities.Payment, int> repo, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Entities.Payment> CreateAsync(Entities.Payment data)
        {
            _context.Add(data);
            _context.SaveChanges();

            return data;
        }

    }
}
