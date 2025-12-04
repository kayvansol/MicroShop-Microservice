using MicroShop.Domain.DTOs.Customer;
using MicroShop.Domain.DTOs.Order;
using MicroShop.Infra.Sql.Repositories.CustomerRepo;
using MicroShop.Infra.Sql.Repositories.OrderSateRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Application.UseCases.Order.Queries
{
    class GetAllWaitingPaymentsQueryHandler : IRequestHandler<GetAllWaitingPaymentsQuery, ResultDto<List<WaitingPaymentDto>>>
    {

        private readonly IOrderStatRepo orderStatRepository;
        private readonly IMapper mapper;

        public GetAllWaitingPaymentsQueryHandler(IOrderStatRepo orderStatRepository, IMapper mapper)
        {
            this.orderStatRepository = orderStatRepository;
            this.mapper = mapper;
        }

        public async Task<ResultDto<List<WaitingPaymentDto>>> Handle(GetAllWaitingPaymentsQuery request, CancellationToken cancellationToken)
        {
            var result = await orderStatRepository.WaitingPayments();

            return ResultDto<List<WaitingPaymentDto>>.ReturnData(result, (int)EnumResponseStatus.OK, (int)EnumResultCode.Success, EnumResultCode.Success.GetDisplayName());
        }
    }
}
