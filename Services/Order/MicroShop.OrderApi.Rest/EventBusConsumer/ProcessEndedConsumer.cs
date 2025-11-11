using EventBus.Messages.Events;
using MassTransit;
using MicroShop.Domain.Enums;
using MicroShop.Infra.Sql.Repositories.OrderRepo;

namespace MicroShop.OrderApi.Rest.EventBusConsumer
{
    public class ProcessEndedConsumer : IConsumer<ProcessEndedEvent>
    {
        private readonly ILogger<ProcessEndedConsumer> _logger;
        
        public ProcessEndedConsumer(ILogger<ProcessEndedConsumer> logger)
        {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
        }


        public async Task Consume(ConsumeContext<ProcessEndedEvent> context)
        {
                       
            _logger.LogInformation("ProcessEndedEvent consumed successfully. OrderId : ", context.Message.OrderId.ToString());

        }
    }
}
