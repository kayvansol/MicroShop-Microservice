using Consul;

namespace MicroShop.Basket.API.Extensions
{
    public static class Consul
    {
        public static async Task<Uri> GetUrlFromConsul(this String uri,string consulServerIP,string consulPort,string serviceName)
        {

            // 1. ایجاد Consul Client
            using var consulClient = new ConsulClient(cfg =>
            {
                cfg.Address = new Uri($"http://{consulServerIP}:{consulPort}"); // آدرس Consul
            });

            // 2. گرفتن لیست سرویس‌های سالم
            var services = await consulClient.Health.Service(serviceName, "", passingOnly: true);

            if (services.Response.Length == 0)
            {
                Console.WriteLine($"سرویس {serviceName} یافت نشد.");
                return null;
            }

            var service = services.Response[0];

            string address = service.Service.Address; // آدرس IP یا Hostname
            int port = service.Service.Port;          // پورت سرویس

            Console.WriteLine($"Service Address: {address}");
            Console.WriteLine($"Service Port: {port}");

            // 4. ایجاد URI کامل
            return new Uri($"http://{address}:{port}");
  
        }

    }

}