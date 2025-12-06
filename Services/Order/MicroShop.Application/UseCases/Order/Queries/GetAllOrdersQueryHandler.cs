
using MicroShop.Domain.DTOs.Order;
using MicroShop.Infra.Sql.Repositories.OrderRepo;
using MicroShop.Infra.Sql.Repositories.OrderSateRepo;

namespace MicroShop.Application.UseCases.Order.Queries
{
    class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, ResultDto<List<GetAllOrders>>>
    {

        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;

        public GetAllOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<ResultDto<List<GetAllOrders>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {

            var result = await orderRepository.GetAllOrders();

            return ResultDto<List<GetAllOrders>>.ReturnData(result, (int)EnumResponseStatus.OK, (int)EnumResultCode.Success, EnumResultCode.Success.GetDisplayName());

        }
    }
}
