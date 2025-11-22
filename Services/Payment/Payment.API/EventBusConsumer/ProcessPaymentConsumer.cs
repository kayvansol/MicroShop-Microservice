using EventBus.Messages.Events;
using MassTransit;
using Payment.API.Enums;
using Payment.API.Repositories.PaymentRepo;

namespace Payment.API.EventBusConsumer
{

    public class ProcessPaymentConsumer : IConsumer<ProcessPayment>
    {        
        private readonly ILogger<ProcessPaymentConsumer> _logger;
        private readonly IPaymentRepository repository;
        public IPublishEndpoint _publishEndpoint { get; }

        public ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger, IPaymentRepository repository, IPublishEndpoint publishEndpoint)
        {            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
            _publishEndpoint = publishEndpoint;
        }


        public async Task Consume(ConsumeContext<ProcessPayment> context)
        {

            Entities.Payment payment = new Entities.Payment()
            {
                OrderId = context.Message.OrderId,
                CardNumber = "5435443689665465",
                PaymentMethod = (short) EnumPaymentMethod.Online
            };

            bool condition = true;

            if (condition) // Can Pay ...
            {
                payment.Status = (short)EnumPaymentState.PaymentSucceeded;

                await repository.CreateAsync(payment);

                await _publishEndpoint.Publish(new PaymentSucceededEvent
                {
                    CorrelationId = context.Message.CorrelationId, // CorrelationId
                    OrderId = context.Message.OrderId,
                    CustomerId = context.Message.CustomerId,
                    Created = context.Message.Created
                });

            }
            else // Can Not Pay ...
            {

                payment.Status = (short)EnumPaymentState.PaymentFailed;

                await repository.CreateAsync(payment);

                await _publishEndpoint.Publish(new PaymentFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId, // CorrelationId
                    OrderId = payment.OrderId,
                    Reason = "Insufficient funds"
                });

            }

        }
    }
}
