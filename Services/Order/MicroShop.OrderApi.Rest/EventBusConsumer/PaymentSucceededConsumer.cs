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
    public class PaymentSucceededConsumer : IConsumer<PaymentSucceededEvent>
    {
        private readonly ILogger<PaymentSucceededConsumer> _logger;
        private readonly IOrderRepository repository;

        public PaymentSucceededConsumer(ILogger<PaymentSucceededConsumer> logger, IOrderRepository repository)
        {
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
        }


        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
        {

            var result = await repository.UpdateStatusAsync(context.Message.OrderId);

            _logger.LogInformation("PaymentSucceededEvent consumed successfully. Created result : ", result);

        }
    }
}
