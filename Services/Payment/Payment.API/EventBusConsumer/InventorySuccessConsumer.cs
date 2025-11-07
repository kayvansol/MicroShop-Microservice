using EventBus.Messages.Events;
using MassTransit;
using Payment.API.Repositories.PaymentRepo;

namespace Payment.API.EventBusConsumer
{

    public class InventorySuccessConsumer : IConsumer<InventorySuccessEvent>
    {        
        private readonly ILogger<InventorySuccessConsumer> _logger;
        private readonly IPaymentRepository repository;
        public IPublishEndpoint _publishEndpoint { get; }

        public InventorySuccessConsumer(ILogger<InventorySuccessConsumer> logger, IPaymentRepository repository, IPublishEndpoint publishEndpoint)
        {            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
            _publishEndpoint = publishEndpoint;
        }


        public async Task Consume(ConsumeContext<InventorySuccessEvent> context)
        {

            Entities.Payment payment = new Entities.Payment()
            {
                OrderId = context.Message.OrderId,
                CardNumber = "5435443689665465",
                PaymentMethod = 1,
                Status = 1
            };

            await repository.CreateAsync(payment);

            // _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id : {newOrderId}", result);

            await _publishEndpoint.Publish<PaymentSucceededEvent>(new
            {
                OrderId = context.Message.OrderId,
                CustomerId = context.Message.CustomerId,
                Created = context.Message.Created
            });

            Thread.Sleep(5000);

        }
    }
}
