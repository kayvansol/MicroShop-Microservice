
namespace MicroShop.Domain.DTOs.Product
{
    public class GetAllProductDto
    {
        public string ProductName { get; set; } = null!;

        public string CategoryName { get; set; }

        public decimal Price { get; set; }

        public int Inventory { get; set; }

    }
}
