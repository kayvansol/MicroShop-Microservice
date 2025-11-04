using Discount.API.Context;

namespace Discount.API.Repositories
{
    public interface ICouponRepository : IRepository<Coupon, int>
    {
        Task<Coupon> GetCouponByProductIdAsync(int ProductId);

        Task<Coupon> CreateAsync(Coupon data);
    }
}
