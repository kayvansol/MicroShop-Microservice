
namespace MicroShop.Domain.DTOs.Order
{
    public record WaitingPaymentDto
    {
        public Guid CorrelationID { get; set; }
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime Created { get; set; }
    }
}
