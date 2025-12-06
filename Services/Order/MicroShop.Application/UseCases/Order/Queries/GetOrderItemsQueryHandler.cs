using MicroShop.Domain.DTOs.Order;
using MicroShop.Infra.Sql.Repositories.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroShop.Application.UseCases.Order.Queries
{
    class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, ResultDto<List<GetOrderItems>>>
    {

        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public GetOrderItemsQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<ResultDto<List<GetOrderItems>>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
        {

            var result = await orderRepository.GetOrderItems(request.OrderId);

            return ResultDto<List<GetOrderItems>>.ReturnData(result, (int)EnumResponseStatus.OK, (int)EnumResultCode.Success, EnumResultCode.Success.GetDisplayName());

        }
    }
    
}
