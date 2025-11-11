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
    public class OrderCanceledConsumer : IConsumer<OrderCanceledEvent>
    {
        private readonly ILogger<OrderCanceledConsumer> _logger;
        private readonly IOrderRepository repository;

        public OrderCanceledConsumer(ILogger<OrderCanceledConsumer> logger, IOrderRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
        }
        

        public async Task Consume(ConsumeContext<OrderCanceledEvent> context)
        {
            
            _logger.LogInformation("OrderCanceledEvent consumed successfully. Created result : ", "");

        }
    }
}
