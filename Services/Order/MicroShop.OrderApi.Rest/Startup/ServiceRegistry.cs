using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using MicroShop.OrderApi.Rest.Attributes;
using MicroShop.OrderApi.Rest.Middlewares;
using MicroShop.OrderApi.Rest.Services;
using MicroShop.OrderApi.Rest.Mapper;
using MicroShop.Infra.Sql.Context;
using Microsoft.Extensions.Options;
using MassTransit;
using MicroShop.OrderApi.Rest.EventBusConsumer;
using MicroShop.OrderApi.Rest.SagaStateMachine;
using System.Text.Json.Serialization;
//using Microsoft.EntityFrameworkCore.InMemory;

namespace MicroShop.OrderApi.Rest.Startup
{
    public static class ServiceRegistry
    {
        public static void Register(this IServiceCollection services, IConfiguration configuration)
        {

            /*services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 80;
            });*/

            #region Public

            services.AddControllers().AddJsonOptions(options =>
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); ;

            services.AddHttpContextAccessor();

            #endregion

            #region MassTransit

            // MassTransit-RabbitMQ Configuration
            services.AddMassTransit(config => {

                config.SetKebabCaseEndpointNameFormatter();

                config.AddConsumer<BasketCheckoutConsumer>();
                config.AddConsumer<PaymentSucceededConsumer>();
                config.AddConsumer<InventoryFailedConsumer>();
                config.AddConsumer<PaymentFailedConsumer>();
                config.AddConsumer<OrderCanceledConsumer>();
                config.AddConsumer<ProcessEndedConsumer>();

                //config.AddSagaStateMachine
                config.AddSagaStateMachine<OrderStateMachine, OrderState>().InMemoryRepository();

                config.UsingRabbitMq((ctx, cfg) => {

                    cfg.Host(configuration["EventBusSettings:HostAddress"]);

                    cfg.UseInMemoryOutbox();

                    /*cfg.ReceiveEndpoint(EventBus.Messages.Common.EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        //c.UseConcurrencyLimit(1);

                        //c.ConcurrentMessageLimit = 1;

                        c.UseInMemoryOutbox();

                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                        c.ConfigureConsumer<PaymentSucceededConsumer>(ctx);
                        c.ConfigureConsumer<InventoryFailedConsumer>(ctx);
                        c.ConfigureConsumer<PaymentFailedConsumer>(ctx);
                        c.ConfigureConsumer<OrderCanceledConsumer>(ctx);
                    });*/




                    /*

                    // Configure a global retry policy
                    // --- Resilient Processing Configuration ---

                    // Stage 2: For longer outages, schedule message for redelivery.
                    // This middleware will take over after the initial retries from UseMessageRetry have failed.
                    cfg.UseDelayedRedelivery(r =>
                        // Configure 5 redelivery attempts with a 10-minute interval between each.
                        //r.Interval(5, TimeSpan.FromMinutes(10)));
                        r.Interval(5, TimeSpan.FromSeconds(3)));

                    // Stage 1: For transient faults, retry the message immediately a few times.
                    cfg.UseMessageRetry(r =>
                        // Retry 3 times with the specified intervals between attempts.
                        r.Intervals(
                            TimeSpan.FromMilliseconds(500), // 1st retry after 0.5s
                            TimeSpan.FromSeconds(5),        // 2nd retry after 5s
                            TimeSpan.FromSeconds(10)        // 3rd retry after 10s
                        ));

                    */

                    // --- End of Configuration ---
                    cfg.ConfigureEndpoints(ctx);

                });
            });
            //services.AddMassTransitHostedService(true);

            // General Configuration
            services.AddScoped<BasketCheckoutConsumer>();
            services.AddScoped<PaymentSucceededConsumer>();
            services.AddScoped<InventoryFailedConsumer>();
            services.AddScoped<PaymentFailedConsumer>();
            services.AddScoped<OrderCanceledConsumer>();
            services.AddScoped<ProcessEndedConsumer>();

            #endregion

            #region Hosted Service

            //services.AddHostedService<GlobalTimer>();
            //services.AddHostedService<GlobalTimer2>();

            #endregion

            #region DbContext

            // Scaffold-DbContext "Data Source=.;Initial Catalog=MicroShopLogDB;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Context -Force

            services.AddDbContext<MicroShopContext>(option => option.UseSqlServer(configuration["ApplicationOptions:StoreConnectionString"]));

            services.AddDbContext<MicroShopLogDbContext>(option => option.UseSqlServer(configuration["ApplicationOptions:LogDBConnectionString"]));

            //services.AddDbContext<MicroShopLogDbContext>(opt => opt.UseInMemoryDatabase("LogDB")); // change also in dbcontext

            services.AddScoped<MicroShopContext>();

            services.AddScoped<MicroShopLogDbContext>();

            #endregion

            #region Validators

            services.AddValidatorsFromAssembly(typeof(MicroShop.Core.InjectCore).GetTypeInfo().Assembly);

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            #endregion

            #region Dependency Injection

            services.AddTransient(typeof(PermissionAttribute));

            services.AddScoped(typeof(LoggingMiddleware));

            services.AddScoped(typeof(ExceptionHandlingMiddleware));

            //services.AddScoped(typeof(ValidationMiddleware<WeatherForecast>));

            services.AddOptions<CustomOptions>().Bind(configuration.GetSection("ApplicationOptions"));

            services.AddScoped(typeof(LoggingBehaviour<,>));

            #endregion

            #region Auto Mapper

            // Auto Mapper Config ...

            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile(new MapperProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);

            #endregion

            #region Authentication & Authorization
            /*
            services.AddAuthorization(c =>
            {
                c.AddPolicy("MyApiPolicy", policy =>
                 {
                     //policy.RequireAuthenticatedUser();
                     policy.RequireClaim("scope", "api_rest");
                 });
            });
            */
            #endregion

            #region Swagger

            // Swagger 
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Protected API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://localhost:7003/connect/authorize"),
                            TokenUrl = new Uri("https://localhost:7003/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                {"api_rest", "Demo API - full access"}
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            #endregion

            

        }

    }

}
