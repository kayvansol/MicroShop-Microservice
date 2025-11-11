using MassTransit;

namespace MicroShop.OrderApi.Rest.SagaStateMachine
{

    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }  // الزامی در MassTransit
        public int OrderId { get; set; }         // شناسه عددی سفارش
        public string CurrentState { get; set; } = default!;
        public int CustomerId { get; set; }
        public DateTime Created { get; set; }
        public string? CancelReason { get; set; }
        public byte[]? RowVersion { get; set; }
    }

}