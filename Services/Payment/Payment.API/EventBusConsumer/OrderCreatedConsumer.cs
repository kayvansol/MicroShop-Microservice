using EventBus.Messages.Events;
using MassTransit;
using MassTransit.Transports;
using Microsoft.Extensions.Logging;
using Payment.API.Repositories.PaymentRepo;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Payment.API.EventBusConsumer
{

    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {        
        private readonly ILogger<OrderCreatedConsumer> _logger;
        private readonly IPaymentRepository repository;
        public IPublishEndpoint _publishEndpoint { get; }

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger, IPaymentRepository repository, IPublishEndpoint publishEndpoint)
        {            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
            _publishEndpoint = publishEndpoint;
        }


        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
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
                Created = context.Message.CreationDate
            });


        }
    }
}
