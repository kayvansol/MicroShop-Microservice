using MicroShop.Domain.DTOs.Order;
using MicroShop.Domain.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Core.Queries
{
    
    public record GetAllWaitingPaymentsQuery : IRequest<ResultDto<List<WaitingPaymentDto>>>
    {

    }
}
