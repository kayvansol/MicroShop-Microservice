using EventBus.Messages.Events;
using MassTransit;
using Payment.API.Enums;
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
                PaymentMethod = (short) EnumPaymentMethod.Online
            };


            if (DateTime.Now.Millisecond > 500) // Can Pay ...
            {
                payment.Status = (short)EnumPaymentState.PaymentSucceeded;

                await repository.CreateAsync(payment);

                await _publishEndpoint.Publish<PaymentSucceededEvent>(new
                {
                    OrderId = context.Message.OrderId
                });

            }
            else // Can Not Pay ...
            {

                payment.Status = (short)EnumPaymentState.PaymentFailed;

                await repository.CreateAsync(payment);

                await _publishEndpoint.Publish<PaymentFailedEvent>(new
                {
                    OrderId = context.Message.OrderId
                });

            }

            Thread.Sleep(5000);

        }
    }
}
