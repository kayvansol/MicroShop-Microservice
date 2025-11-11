using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MassTransit.Transports;
using MediatR;
using MicroShop.Domain.DTOs.Order;
using MicroShop.Domain.Enums;
using MicroShop.Infra.Sql.Repositories.OrderRepo;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroShop.OrderApi.Rest.EventBusConsumer
{
    public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
    {
        private readonly ILogger<PaymentSucceededConsumer> _logger;
        private readonly IOrderRepository repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentSucceededConsumer(ILogger<PaymentSucceededConsumer> logger, IOrderRepository repository, IPublishEndpoint publishEndpoint)
        {
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
            this._publishEndpoint = publishEndpoint;
        }


        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
        {

            // پرداخت شده
            var result = await repository.UpdateStatusAsync(context.Message.OrderId, EnumOrderState.Paid); 


            // آپدیت و کسر موجودی کالاها
            var res = await repository.UpdateInventoriesAsync(context.Message.OrderId);


            await _publishEndpoint.Publish(new ProcessEndedEvent
            {
                CorrelationId = context.Message.CorrelationId, // CorrelationId
                OrderId = context.Message.OrderId
            });


            _logger.LogInformation("PaymentSucceededEvent consumed successfully. Created result : ", result);

        }
    }
}
