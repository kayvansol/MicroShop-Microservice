using System.Collections.Generic;

namespace Basket.API.Entities
{
    public class ShoppingCart
    {
        public int CustomerId { get; set; }
        public IList<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public ShoppingCart()
        {
        }

        public ShoppingCart(int customerId)
        {
            CustomerId = customerId;
        }

        public decimal TotalPrice
        {
            get
            {
                decimal totalprice = 0;

                foreach (var item in Items)
                {
                    totalprice += item.Price * item.Quantity;
                }
                
                return totalprice;
            }
        }
    }
}