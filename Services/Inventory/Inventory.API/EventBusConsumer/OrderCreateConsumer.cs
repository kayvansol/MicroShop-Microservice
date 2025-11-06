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
                // InventoryFailedEvent
            }
            else
            {

                await _publishEndpoint.Publish<InventorySuccessEvent>(new
                {
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Message.CustomerId,
                    Created = context.Message.Created
                });


                Thread.Sleep(5000);

            }

            //return Task.CompletedTask;
        }
    }
}
