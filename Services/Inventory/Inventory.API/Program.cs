using Inventory.API.EventBusConsumer;
using Inventory.API.Repositories;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

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

    config.AddConsumer<OrderCreateConsumer>();

    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.ReceiveEndpoint(EventBus.Messages.Common.EventBusConstants.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<OrderCreateConsumer>(ctx);
        });
    });
});
builder.Services.AddMassTransitHostedService();

// General Configuration
builder.Services.AddScoped<OrderCreateConsumer>();

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

app.Run();
