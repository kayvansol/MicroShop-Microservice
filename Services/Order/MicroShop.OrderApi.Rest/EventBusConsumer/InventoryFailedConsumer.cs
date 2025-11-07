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
    public class InventoryFailedConsumer : IConsumer<InventoryFailedEvent>
    {
        private readonly ILogger<InventoryFailedConsumer> _logger;
        private readonly IOrderRepository repository;

        public InventoryFailedConsumer(ILogger<InventoryFailedConsumer> logger, IOrderRepository repository)
        {
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
        }


        public async Task Consume(ConsumeContext<InventoryFailedEvent> context)
        {

            var result = await repository.UpdateStatusAsync(context.Message.OrderId, EnumOrderState.OutOfStock); //عدم موجودی 

            _logger.LogInformation("InventoryFailedEvent consumed successfully. Created result : ", result);

        }
    }
}
