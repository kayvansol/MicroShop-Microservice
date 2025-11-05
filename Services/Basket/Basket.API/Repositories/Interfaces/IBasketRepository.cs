using Basket.API.Entities;
using System.Threading.Tasks;

namespace Basket.API.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(int CustomerId);
        Task<ShoppingCart> UpdateBasket(ShoppingCart basket);
        Task<bool> DeleteBasket(int CustomerId);
    }
}
