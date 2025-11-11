using MassTransit;
using EventBus.Messages.Events;
using Inventory.API.Repositories;

namespace Inventory.API.EventBusConsumer
{
    public class OrderCreateConsumer : IConsumer<OrderCreateEvent>
    {
        private readonly ILogger<OrderCreateConsumer> _logger;
        private readonly IInventoryRepository repository;
        public IPublishEndpoint _publishEndpoint { get; }

        public OrderCreateConsumer(ILogger<OrderCreateConsumer> logger, IInventoryRepository repository, IPublishEndpoint publishEndpoint)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreateEvent> context)
        {

            var IsThereEmptyInventory = await repository.IsThereEmptyInventory(context.Message.OrderId);

            if (IsThereEmptyInventory)
            {
                
                await _publishEndpoint.Publish(new InventoryFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId, // CorrelationId
                    OrderId = context.Message.OrderId,
                    Reason = "Item out of stock"
                });


                //Thread.Sleep(5000);

            }
            else
            {

                await _publishEndpoint.Publish(new InventorySuccessEvent
                {
                    CorrelationId = context.Message.CorrelationId, // CorrelationId
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Message.CustomerId,
                    Created = context.Message.Created
                });


                //Thread.Sleep(5000);

            }

            //return Task.CompletedTask;
        }
    }
}
