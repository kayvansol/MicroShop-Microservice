using Basket.API.gRPCServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Repositories;
using Discount.gRPC.Protos;
using MassTransit;
using Steeltoe.Discovery.Client;
using Consul;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Grpc.Net.Client;
using MicroShop.Basket.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddHealthChecks();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5292, o =>
    {
        o.Protocols = HttpProtocols.Http1AndHttp2;   // گارانتی قطعی
        //o.UseHttps();
    });
});

// General Configuration
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);

#region gRPC Configuration

string IsDevelopment = builder.Configuration["gRPCSettings:IsDevelopment"];

// gRPC Configuration
Uri serviceUri = null;

if (IsDevelopment != "True")
{
    serviceUri = new Uri(builder.Configuration["gRPCSettings:DiscountUrl"]);    
}
else
{
    string consulServerIP = builder.Configuration["gRPCSettings:ConsulServerIP"];
    
    serviceUri = await "GetUri".GetUrlFromConsul(consulServerIP,"8500","discount-service");
}

#region Add Grpc Client & Test gRPC method

if(serviceUri != null)
{
    builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
        (o => o.Address = serviceUri);

    // ایجاد gRPC channel
    using var channel = GrpcChannel.ForAddress(serviceUri);
    var client = new DiscountProtoService.DiscountProtoServiceClient(channel);

    var reply = await client.GetDiscountAsync(new Discount.gRPC.Protos.GetDiscountRequest { ProductId = "4" });

    Console.WriteLine($"Product: {reply.ProductID}, Discount: {reply.Amount}");
}

#endregion

builder.Services.AddScoped<DiscountgRPCService>();

#endregion

#region MassTransit

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.UseInMemoryOutbox();

        cfg.ConfigureEndpoints(ctx);

    });
});
//builder.Services.AddMassTransitHostedService();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// HealthCheck endpoint for Consul
app.MapHealthChecks("/health");

// Register to Consul
app.UseDiscoveryClient();

app.Run();
