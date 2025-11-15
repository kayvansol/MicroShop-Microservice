namespace MicroShop.OrderApi.Rest.Data
{
    public class OrderEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int OrderId { get; set; }
        public string EventType { get; set; } = default!;
        public string? EventData { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
