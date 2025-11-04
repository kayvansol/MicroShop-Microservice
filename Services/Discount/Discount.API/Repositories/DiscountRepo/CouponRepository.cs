using Discount.API.Context;
using Microsoft.EntityFrameworkCore;


namespace Discount.API.Repositories;

public class CouponRepository : Repository<Coupon, int>, ICouponRepository
{
    private readonly MicroShopDiscountContext _context;
    private readonly IRepository<Coupon, int> _repo;
    
    public CouponRepository(MicroShopDiscountContext context,
        IRepository<Coupon, int> repo) : base(context)
    {
        _context = context;
        _repo = repo;
    }

    public async Task<Coupon> GetCouponByProductIdAsync(int ProductId)
    {
        var list = _repo.GetAll(predicate: x => x.ProductId == ProductId);

        var result = list.FirstOrDefault();

        return result;
    }

    public async Task<Coupon> CreateAsync(Coupon data)
    {
        _context.Add(data);
        _context.SaveChanges();

        return data;
    }
}
