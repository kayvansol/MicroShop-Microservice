using MicroShop.Domain.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Infra.Sql.Repositories.OrderSateRepo
{
    public interface IOrderStatRepo
    {
        public Task<List<WaitingPaymentDto>> WaitingPayments();
    }
}
