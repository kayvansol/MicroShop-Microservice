using MassTransit;
using MicroShop.OrderApi.Rest.Data;
using System.Text.Json;

namespace MicroShop.OrderApi.Rest.SagaStateMachine
{
    public class GenericOrderEventActivity<T> : IStateMachineActivity<OrderState, T>
        where T : class
    {
        private readonly IOrderEventStoreService _eventStore;

        public GenericOrderEventActivity(IOrderEventStoreService eventStore)
        {
            _eventStore = eventStore;
        }

        public void Probe(ProbeContext context) { }
        public void Accept(StateMachineVisitor visitor) { }

        public async Task Execute(BehaviorContext<OrderState, T> context, IBehavior<OrderState, T> next)
        {
            var orderIdProp = context.Data.GetType().GetProperty("OrderId");
            if (orderIdProp != null)
            {
                var orderId = (int)orderIdProp.GetValue(context.Data)!;

                await _eventStore.AppendAsync(new OrderEvent
                {
                    OrderId = orderId,
                    EventType = typeof(T).Name,
                    EventData = System.Text.Json.JsonSerializer.Serialize(context.Data),
                    CreatedAt = DateTime.Now
                });
            }

            await next.Execute(context);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, T, TException> context, IBehavior<OrderState, T> next)
            where TException : Exception
            => next.Faulted(context);
    }
}
