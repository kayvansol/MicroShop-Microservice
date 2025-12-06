using MicroShop.Domain.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Core.Queries
{
    public class GetOrderItemsQuery : IRequest<ResultDto<List<GetOrderItems>>>
    {
        public int OrderId { get; set; }
    }
}
