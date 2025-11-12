using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroShop.OrderApi.Rest.SagaStateMachine
{
    public class OrderStateMap : SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.CancelReason).HasMaxLength(256);
            entity.HasIndex(x => x.OrderId).IsUnique(true);
            entity.Property(x => x.CustomerId);
            entity.Property(x => x.Created);
        }
    }
}