using MassTransit;
using EventBus.Messages.Events;
using MicroShop.OrderApi.Rest.Data;

namespace MicroShop.OrderApi.Rest.SagaStateMachine
{

    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {

        public State ProcessingInventory { get; set; }

        public State ProcessingPayment { get; set; }

        public State ProcessingEnd { get; set; }

        public State Shipped { get; set; }

        public State Canceled { get; set; }


        public Event<BasketCheckoutEvent> BasketCheckoutEvent { get; set; }

        public Event<OrderCreateEvent> OrderCreateEvent { get; set; }

        public Event<InventorySuccessEvent> InventorySuccessEvent { get; set; }

        public Event<InventoryFailedEvent> InventoryFailedEvent { get; set; }

        public Event<PaymentSucceededEvent> PaymentSucceededEvent { get; set; }

        public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }

        public Event<ProcessEndedEvent> ProcessEndedEvent { get; set; }

        //public Event<IOrderShipped> OrderShipped { get; private set; }

        public OrderStateMachine()
        {

            InstanceState(x => x.CurrentState);

            Event(() => BasketCheckoutEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => OrderCreateEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => InventorySuccessEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => InventoryFailedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => PaymentSucceededEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => PaymentFailedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => ProcessEndedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            //Event(() => OrderShipped, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));


            //*******************************************************************


            // مرحله 1: ایجاد رکورد سفارش        
            Initially(
                When(OrderCreateEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Order {c.Data.OrderId} Created by CustomerId : {c.Data.CustomerId}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<OrderCreateEvent>>())
                    .PublishAsync(async c => new ProcessInventory()
                    {
                        OrderId = c.Message.OrderId,
                        CustomerId = c.Message.CustomerId,
                        CorrelationId = c.Message.CorrelationId,
                        Created = c.Message.Created
                    })
                    .TransitionTo(ProcessingInventory)
                    .Then(c => Log.Information($"[Saga] Transitioned to ProcessingInventory for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 2: بررسی موجودی        
            During(ProcessingInventory,
                When(InventorySuccessEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Inventory reserved for Order {c.Data.OrderId}");
                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<InventorySuccessEvent>>())
                    .PublishAsync(async c => new ProcessPayment()
                    {
                        OrderId = c.Message.OrderId,
                        CustomerId = c.Message.CustomerId,
                        CorrelationId = c.Message.CorrelationId,
                        Created = c.Message.Created
                    })
                    .TransitionTo(ProcessingPayment)
                    .Then(c => Log.Information($"[Saga] Transitioned to ProcessingPayment for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}")),

                When(InventoryFailedEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.CancelReason = c.Data.Reason;
                        Log.Information($"Inventory reservation failed for Order {c.Data.OrderId}: {c.Data.Reason}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<InventoryFailedEvent>>())
                    //.Publish(c => new OrderCanceledEvent() { OrderId = c.Data.OrderId, Reason = c.Data.Reason })
                    .TransitionTo(Canceled)
                    .Then(c => Log.Information($"[Saga] Transitioned to Canceled for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 3: پرداخت موفق
            During(ProcessingPayment,
                When(PaymentSucceededEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Payment done for Order {c.Data.OrderId}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<PaymentSucceededEvent>>())
                    .PublishAsync(async c => new ProcessEnd()
                    {
                        OrderId = c.Message.OrderId,
                        CustomerId = c.Message.CustomerId,
                        CorrelationId = c.Message.CorrelationId,
                        Created = c.Message.Created
                    })
                    .Then(c => Log.Information($"[Saga] Transitioned to ProcessingEnd for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
                    .TransitionTo(ProcessingEnd),


                When(PaymentFailedEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.CancelReason = c.Data.Reason;
                        Log.Information($"Payment failed for Order {c.Data.OrderId}: {c.Data.Reason}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<PaymentFailedEvent>>())
                    //.Publish(c => new OrderCanceledEvent { OrderId = c.Data.OrderId, Reason = c.Data.Reason })
                    .TransitionTo(Canceled)
                    .Then(c => Log.Information($"[Saga] Transitioned to Canceled for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))

            );

            // مرحله 4: end
            During(ProcessingEnd,
                When(ProcessEndedEvent)
                    .ThenAsync(async c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        //c.Instance.CustomerId = c.Data.CustomerId;
                        //c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Ending done for Order {c.Data.OrderId}");

                    })
                    .Activity(x => x.OfType<GenericOrderEventActivity<ProcessEndedEvent>>())
                    .TransitionTo(Shipped)
                    .Then(c => Log.Information($"[Saga] Transitioned to Shipped for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
                    //.Publish(c => new { c.Message.OrderId } as IShipOrder),
                    .Finalize()

            );


            // Log any unexpected incoming events for debugging
            DuringAny(
                When(ProcessEndedEvent)
                    .Activity(x => x.OfType<GenericOrderEventActivity<ProcessEndedEvent>>())
                    .Then(ctx => Console.WriteLine($"⚠️ ProcessEndedEvent received in unexpected state           {ctx.Saga.CurrentState}"))
                  //.TransitionTo(ProcessEnded)
            );

            SetCompletedWhenFinalized();

            Log.Information("✅ OrderStateMachine constructor called");

        }
    }

}