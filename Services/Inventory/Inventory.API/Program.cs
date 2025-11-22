using Inventory.API.EventBusConsumer;
using Inventory.API.Repositories;
using MassTransit;
using Steeltoe.Discovery.Client;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5104, o =>
    {
        o.Protocols = HttpProtocols.Http1AndHttp2;   // گارانتی قطعی
        //o.UseHttps();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

builder.Logging.AddConsole();

#region MassTransit

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config => {

    config.AddConsumer<ProcessInventoryConsumer>();

    config.UsingRabbitMq((ctx, cfg) => {

        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.UseInMemoryOutbox();

        cfg.ConfigureEndpoints(ctx);

    });
});

// General Configuration
builder.Services.AddScoped<ProcessInventoryConsumer>();

#endregion

builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddHealthChecks();

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
