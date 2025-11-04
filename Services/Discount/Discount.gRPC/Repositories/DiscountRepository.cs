using Azure.Core;
using Microsoft.Data.SqlClient;

namespace Discount.gRPC.Repositories
{
    public class DiscountRepository
    {
        public async Task<Discount.gRPC.Protos.CouponModel> GetDiscount(int ProductId)
        {

            string connectionString = "Data Source=192.168.1.4;Initial Catalog=MicroShopDiscount;User ID=sa;Password=ABCabc123456;TrustServerCertificate=True";

            Discount.gRPC.Protos.CouponModel result = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT TOP (1) [id],[ProductID],[Amount],[Description] FROM [MicroShopDiscount].[dbo].[Coupon] where ProductID = @ProductID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", ProductId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result = new Discount.gRPC.Protos.CouponModel
                            {
                                Id = reader.GetInt32(0),
                                ProductID = reader.GetInt32(1),
                                Amount = reader.GetInt32(2),
                                Description = reader.GetString(3)
                            };
                        }
                    }
                }

            }

            return result;
        }
    }
}
