using MassTransit;
using EventBus.Messages.Events;

namespace MicroShop.OrderApi.Rest.SagaStateMachine
{

    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {

        public State Checkout { get; set; }

        public State Created { get; set; }

        public State InventoryReserved { get; set; }

        public State Paid { get; set; }

        public State Shipped { get; set; }
        public State ProcessEnded { get; set; }

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

            /*
            Event(() => OrderCreateEvent, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId).SelectId(c => Guid.NewGuid()));
            Event(() => InventorySuccessEvent, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));
            Event(() => InventoryFailedEvent, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));
            Event(() => PaymentSucceededEvent, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));
            Event(() => PaymentFailedEvent, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));
            //Event(() => OrderShipped, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));
            */

            Event(() => BasketCheckoutEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => OrderCreateEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => InventorySuccessEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => InventoryFailedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => PaymentSucceededEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => PaymentFailedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            Event(() => ProcessEndedEvent, e => e.CorrelateBy((s, c) => s.CorrelationId == c.Message.CorrelationId));

            //Event(() => OrderShipped, e => e.CorrelateBy((s, c) => s.OrderId == c.Message.OrderId));

            // شروع جریان کاری
            Initially(
                When(BasketCheckoutEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = 0;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"CorrelationId {c.Instance.CorrelationId} submitted by CustomerId : {c.Data.CustomerId}");
                    })
                    //.Publish(c => new { c.Message.OrderId, c.Message.CustomerId } as IReserveInventory) // اختیاری
                    .TransitionTo(Checkout)
                    .Then(c => Log.Information($"[Saga] Transitioned to Checkout for CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 1: ایجاد رکورد سفارش        
            During(Checkout,
                When(OrderCreateEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Order {c.Data.OrderId} submitted by CustomerId : {c.Data.CustomerId}");
                    })
                    //.Publish(c => new { c.Message.OrderId, c.Message.CustomerId } as IReserveInventory) // اختیاری
                    .TransitionTo(Created)
                    .Then(c => Log.Information($"[Saga] Transitioned to Created for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 2: بررسی موجودی        
            During(Created,
                When(InventorySuccessEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Inventory reserved for Order {c.Data.OrderId}");
                    })
                    //.Publish(c => new{ c.Message.OrderId,c.Message.CustomerId,c.Message.Created} as IPaymentRequest)
                    .TransitionTo(InventoryReserved)
                    .Then(c => Log.Information($"[Saga] Transitioned to InventoryReserved for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}")),

                When(InventoryFailedEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.CancelReason = c.Data.Reason;
                        Log.Information($"Inventory reservation failed for Order {c.Data.OrderId}: {c.Data.Reason}");
                    })
                    //.Publish(c => new OrderCanceledEvent() { OrderId = c.Data.OrderId, Reason = c.Data.Reason })
                    .TransitionTo(Canceled)
                    .Then(c => Log.Information($"[Saga] Transitioned to Canceled for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
            );

            // مرحله 3: پرداخت موفق
            During(InventoryReserved,
                When(PaymentSucceededEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        c.Instance.CustomerId = c.Data.CustomerId;
                        c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Payment done for Order {c.Data.OrderId}");
                    })
                    .Then(c => Log.Information($"[Saga] Transitioned to Paid for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
                    .TransitionTo(Paid),
                //.Publish(c => new { c.Message.OrderId } as IShipOrder),
                //.Finalize(),

                When(PaymentFailedEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.CancelReason = c.Data.Reason;
                        Log.Information($"Payment failed for Order {c.Data.OrderId}: {c.Data.Reason}");
                    })
                    //.Publish(c => new OrderCanceledEvent { OrderId = c.Data.OrderId, Reason = c.Data.Reason })
                    .TransitionTo(Canceled)
                    .Then(c => Log.Information($"[Saga] Transitioned to Canceled for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))

            );

            // مرحله 4: end
            During(Paid,
                When(ProcessEndedEvent)
                    .Then(c =>
                    {
                        c.Instance.CorrelationId = c.Data.CorrelationId; // CorrelationId
                        c.Instance.OrderId = c.Data.OrderId;
                        //c.Instance.CustomerId = c.Data.CustomerId;
                        //c.Instance.Created = c.Data.CreationDate;

                        Log.Information($"Payment done for Order {c.Data.OrderId}");
                    })
                    .TransitionTo(ProcessEnded)
                    .Then(c => Log.Information($"[Saga] Transitioned to Paid for OrderId={c.Instance.OrderId}, CorrelationId={c.Instance.CorrelationId}"))
                    //.Publish(c => new { c.Message.OrderId } as IShipOrder),
                    .Finalize()

            );

            /*
            // مرحله 5: ارسال کالا
            During(Paid,
                When(OrderShipped)
                    .Then(c => Log.Information($"Order {c.Message.OrderId} shipped!"))
                    .Finalize()
            );
            */


            // Log any unexpected incoming events for debugging

            DuringAny(
                When(OrderCreateEvent)
                    .Then(c => Log.Information($"[Saga][DuringAny] OrderCreateEvent received for OrderId={c.Data.OrderId}, CorrelationId={c.Data.CorrelationId}"))
            );


            DuringAny(
                When(PaymentSucceededEvent)
                    .Then(ctx => {
                        Log.Information("[Saga][DuringAny] PaymentSucceededEvent received: OrderId=    {   OrderId},  CorrelationId={CorrelationId}",
                             ctx.Data.OrderId, ctx.Data.CorrelationId);
                        }
                    )
            );


            SetCompletedWhenFinalized();

            Log.Information("✅ OrderStateMachine constructor called");

        }
    }

}