using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Domain.DTOs.Order
{
    public class GetAllOrders
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string OrderStatusName { get; set; }
        public byte OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
    }
}
