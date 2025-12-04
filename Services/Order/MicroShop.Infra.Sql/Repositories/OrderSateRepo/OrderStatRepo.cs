using MicroShop.Domain;
using MicroShop.Domain.DTOs.Order;
using Microsoft.Data.SqlClient;

namespace MicroShop.Infra.Sql.Repositories.OrderSateRepo
{
    public class OrderStatRepo : IOrderStatRepo
    {

        public async Task<List<WaitingPaymentDto>> WaitingPayments()
        {

            string connectionString = "Data Source=192.168.1.4;Initial Catalog=MicroShop;User ID=sa;Password=ABCabc123456;TrustServerCertificate=True";

            List<WaitingPaymentDto> result = new List<WaitingPaymentDto>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT [CorrelationId],[OrderId],[CustomerId],[Created] FROM [MicroShop].[dbo].[OrderState]";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new()
                            {
                                CorrelationID = reader.GetGuid(0),
                                OrderID = reader.GetInt32(1),
                                CustomerID = reader.GetInt32(2),                                
                                Created = reader.GetDateTime(3)
                            });
                        }
                    }
                }

            }

            return result;

        }

    }
}
