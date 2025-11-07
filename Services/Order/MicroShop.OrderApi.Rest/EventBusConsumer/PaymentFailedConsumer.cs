using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using MicroShop.Domain.DTOs.Order;
using MicroShop.Domain.Enums;
using MicroShop.Infra.Sql.Repositories.OrderRepo;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MicroShop.OrderApi.Rest.EventBusConsumer
{
    public class PaymentFailedConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly ILogger<PaymentFailedConsumer> _logger;
        private readonly IOrderRepository repository;

        public PaymentFailedConsumer(ILogger<PaymentFailedConsumer> logger, IOrderRepository repository)
        {
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
        }


        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {

            var result = await repository.UpdateStatusAsync(context.Message.OrderId, EnumOrderState.PaymentFailed); //پرداخت ناموفق 

            _logger.LogInformation("PaymentFailedEvent consumed successfully. Created result : ", result);

        }
    }
}
