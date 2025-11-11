using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MassTransit.Transports;
using MediatR;
using MicroShop.Domain;
using MicroShop.Domain.DTOs.Order;
using MicroShop.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MicroShop.OrderApi.Rest.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketCheckoutConsumer> _logger;


        public BasketCheckoutConsumer(IMediator mediator, IMapper mapper, ILogger<BasketCheckoutConsumer> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            //var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
            //var command = _mapper.Map<AddOrderCommand>(context.Message);

            //Note : context.Message (BasketCheckoutEvent) --> AddOrderCommand (AddOrderCommandDto)

            var BasketItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(context.Message.BasketItems);


            List<Items> items = new List<Items>();

            foreach (var item in BasketItems)
            {
                items.Add(new Items()
                {
                    ProductId = int.Parse(item.ProductId),
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Discount = item.Discount
                });
            }


            AddOrderCommand command = new AddOrderCommand(new Domain.DTOs.Order.AddOrderCommandDto(){

                CustomerId = context.Message.CustomerId,
                Items = items,
                OrderDate = DateTime.Now,
                OrderStatus = (byte)EnumOrderState.Pending, // در انتظار پرداخت
                RequiredDate = DateTime.Now.AddDays(2),
                ShippedDate = DateTime.Now.AddDays(3)
            });


            var result = await _mediator.Send(command);

            var _order = result.Data;


            await context.Publish(new OrderCreateEvent
            {
                CorrelationId = context.Message.CorrelationId, // CorrelationId
                OrderId = _order?.OrderId ?? 0,
                CustomerId = _order?.CustomerId ?? 0,
                Created = _order?.OrderDate ?? DateTime.Now
            });
            

            _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id : {newOrderId}", result);

        }
    }
}
