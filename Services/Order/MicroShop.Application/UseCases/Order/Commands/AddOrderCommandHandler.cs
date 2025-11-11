using EventBus.Messages.Events;
using MassTransit;
using MicroShop.Domain.Entities;
using MicroShop.Infra.Sql.Repositories.OrderRepo;

namespace MicroShop.Application.UseCases.Order.Commands
{
    public class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, ResultDto<OrderDto>>
    {

        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        //public IPublishEndpoint _publishEndpoint { get; }

        public AddOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper
            //, IPublishEndpoint publishEndpoint
            )
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            //_publishEndpoint = publishEndpoint;
        }

        public async Task<ResultDto<OrderDto>> Handle(AddOrderCommand request, CancellationToken cancellationToken)
        {
            var dto = request.AddDto;

            List<Domain.OrderItem> orderItems = new List<Domain.OrderItem>();

            int i = 0;

            foreach (var item in dto.Items)
            {
                i++;

                orderItems.Add(new Domain.OrderItem
                {
                    Discount = item.Discount,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ItemId = i                    
                });
            }

            Domain.Order order = new()
            {
                CustomerId = dto.CustomerId,
                OrderDate = dto.OrderDate,
                RequiredDate = dto.RequiredDate,
                ShippedDate = dto.ShippedDate,
                OrderStatus = (byte) EnumOrderState.Pending, // در انتظار پرداخت
                OrderItems = orderItems
            };

            var _order = await orderRepository.CreateAsync(order);

            OrderDto orderDto = new OrderDto
            {
                OrderId = _order.OrderId,
                CustomerId = _order.CustomerId,
                OrderDate = _order.OrderDate,
                OrderStatus = _order.OrderStatus,
                RequiredDate = _order.RequiredDate,
                ShippedDate = _order.ShippedDate
            };

            /*
            await _publishEndpoint.Publish<OrderCreateEvent>(new 
            {
                OrderId = _order.OrderId,
                CustomerId = _order.CustomerId,
                Created = _order.OrderDate
            }, cancellationToken);
            */

            //Thread.Sleep(5000);


            return ResultDto<OrderDto>.ReturnData(orderDto, (int)EnumResponseStatus.OK, (int)EnumResultCode.Success, EnumResultCode.Success.GetDisplayName());
        }
    }
}
