
namespace MicroShop.Domain.DTOs.Product
{
    public class GetAllProductDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string CategoryName { get; set; }

        public decimal Price { get; set; }

        public int Inventory { get; set; }

        public int? Discount { get; set; }

    }
}
