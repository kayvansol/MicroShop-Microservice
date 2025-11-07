using Microsoft.Data.SqlClient;

namespace Inventory.API.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {

        public async Task<bool> IsThereEmptyInventory(int OrderId)
        {
            
            string connectionString = "Data Source=192.168.1.4;Initial Catalog=MicroShop;User ID=sa;Password=ABCabc123456;TrustServerCertificate=True";

            int inventory = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT COUNT(production.products.product_id) AS Count FROM            production.products INNER JOIN sales.order_items ON production.products.product_id = sales.order_items.product_id WHERE (production.products.inventory - sales.order_items.quantity <= 0) AND (sales.order_items.order_id = @OrderId)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", OrderId);

                    inventory = (int) await command.ExecuteScalarAsync();
                    
                }

            }

            return (inventory > 0);

        }

    }
}
