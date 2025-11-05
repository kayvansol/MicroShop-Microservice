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

            services.AddControllers();

            services.AddHttpContextAccessor();

            #endregion

            #region Hosted Service

            services.AddHostedService<GlobalTimer>();
            services.AddHostedService<GlobalTimer2>();

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

            #region MassTransit

            // MassTransit-RabbitMQ Configuration
            services.AddMassTransit(config => {
                
                config.AddConsumer<BasketCheckoutConsumer>();
                config.AddConsumer<PaymentSucceededConsumer>();

                config.UsingRabbitMq((ctx, cfg) => {
                    cfg.Host(configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint(EventBus.Messages.Common.EventBusConstants.BasketCheckoutQueue, c =>
                    {
                        c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                        c.ConfigureConsumer<PaymentSucceededConsumer>(ctx);
                    });
                });
            });
            services.AddMassTransitHostedService();

            // General Configuration
            services.AddScoped<BasketCheckoutConsumer>();
            services.AddScoped<PaymentSucceededConsumer>();

            #endregion

        }

    }

}
