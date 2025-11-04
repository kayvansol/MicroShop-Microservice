using Discount.API.Context;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Discount.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository couponRepository;

        public CouponController(ICouponRepository couponRepository)
        {
            this.couponRepository = couponRepository;
        }

        [HttpPost("GetCouponByProductId")]
        public async Task<Coupon> GetCouponByProductIdAsync(int ProductId)
        {

            var res = await couponRepository.GetCouponByProductIdAsync(ProductId);

            return res;
        }

        [HttpPost("Create")]
        public async Task<Coupon> CreateAsync(Coupon data)
        {

            var res = await couponRepository.CreateAsync(data);

            return res;
        }
    }
}
