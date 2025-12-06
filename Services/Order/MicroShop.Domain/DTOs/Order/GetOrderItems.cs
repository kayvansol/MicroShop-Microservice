using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Domain.DTOs.Order
{
    public class GetOrderItems
    {
        public int itemId { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal discount { get; set; }
    }
}
