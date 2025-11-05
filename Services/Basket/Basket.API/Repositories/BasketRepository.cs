using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public async Task<ShoppingCart> GetBasket(int CustomerId)
        {
            var basket = await _redisCache.GetStringAsync(CustomerId.ToString());

            if (String.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
        {
            await _redisCache.SetStringAsync(basket.CustomerId.ToString(), JsonConvert.SerializeObject(basket));
            return await GetBasket(basket.CustomerId);
        }

        public async Task<bool> DeleteBasket(int CustomerId)
        {
            try
            {
                await _redisCache.RemoveAsync(CustomerId.ToString());
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
