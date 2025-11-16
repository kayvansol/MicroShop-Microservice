using MicroShop.OrderApi.Rest.Data;
using MicroShop.OrderApi.Rest.EventBusConsumer;
using MicroShop.OrderApi.Rest.SagaStateMachine;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MassTransit;

namespace MicroShop.OrderApi.Rest.Startup
{
    public static class Extentions
    {

        public static void AddMessageBus
        (this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
        {

            #region Event Store

            services.AddDbContext<OrderEventStoreDbContext>(options =>
                        options.UseSqlServer(configuration["ApplicationOptions:OrderEventStoreConnection"]));

            services.AddScoped<IOrderEventStoreService, OrderEventStoreService>();

            /*

                  Add-Migration InitialOrderEventStoreMigration -c OrderEventStoreDbContext 
                        
                  Update-Database -Context OrderEventStoreDbContext

            */

            #endregion

            #region MassTransit

            services.AddDbContext<OrderStateDbContext>(options =>
            {
                options.UseSqlServer(configuration["ApplicationOptions:StoreConnectionString"]);
            });

            // MassTransit-RabbitMQ Configuration
            services.AddMassTransit(config => {

                config.SetKebabCaseEndpointNameFormatter();


                if (assembly != null)
                    config.AddConsumers(assembly);


                //config.AddSagaStateMachine
                config.AddSagaStateMachine<OrderStateMachine, OrderState>().EntityFrameworkRepository(r =>
                {
                    r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // 🔒 جلوگیری از دسترسی همزمان

                    /*  # migrations :

                        Add-Migration InitOrderSaga -c OrderStateDbContext

                        Update-Database -Context OrderStateDbContext

                    */

                    r.AddDbContext<DbContext, OrderStateDbContext>((provider, optionsBuilder) =>
                    {
                        optionsBuilder.UseSqlServer(configuration["ApplicationOptions:StoreConnectionString"],
                                m => m.MigrationsAssembly(typeof(OrderStateDbContext).Assembly.FullName));
                    });


                });

                // خطوط زیر به دلیل پابلیش نشدن پیام در کنترلر پرداخت بعد از اینکه مرحله پرداخت به بعد به دکمه ی پرداخت انتقال یافت یعنی از حالت پردازش خودکار توسط ماشین استیت به حالت دستی
                /*
                // فعال‌سازی Outbox
                config.AddEntityFrameworkOutbox<OrderStateDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(10);
                    o.DuplicateDetectionWindow = TimeSpan.FromMinutes(1);
                    o.UseBusOutbox(); // ✅ پیام‌ها بعد از Commit ارسال می‌شن
                });
                */


                // Register activities (generic). Use one example type so it scans the namespace.
                config.AddActivitiesFromNamespaceContaining<GenericOrderEventActivity<object>>();

                config.UsingRabbitMq((ctx, cfg) => {

                    cfg.Host(configuration["EventBusSettings:HostAddress"]);

                    cfg.UseDelayedRedelivery(r =>
                        
                        r.Interval(5, TimeSpan.FromSeconds(3)));

                    cfg.UseMessageRetry(r =>
                        // Retry 3 times with the specified intervals between attempts.
                        r.Intervals(
                            TimeSpan.FromMilliseconds(500), // 1st retry after 0.5s
                            TimeSpan.FromSeconds(5),        // 2nd retry after 5s
                            TimeSpan.FromSeconds(10)        // 3rd retry after 10s
                        ));

                    // --- End of Configuration ---
                    cfg.ConfigureEndpoints(ctx);

                });
            });
            
            #endregion

        }

    }
}