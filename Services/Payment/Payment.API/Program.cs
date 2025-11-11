using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.API.Context;
using Payment.API.EventBusConsumer;
using Payment.API.Mapper;
using Payment.API.Repositories;
using Payment.API.Repositories.PaymentRepo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext

builder.Services.AddDbContext<MicroShopPaymentContext>(option => option.UseSqlServer(builder.Configuration["ApplicationOptions:StoreConnectionString"]));

builder.Services.AddScoped<MicroShopPaymentContext>();

builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

#endregion

#region MassTransit

// MassTransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config => {

    config.AddConsumer<InventorySuccessConsumer>();

    config.UsingRabbitMq((ctx, cfg) => {

        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);

        cfg.UseInMemoryOutbox();

        cfg.ConfigureEndpoints(ctx);

    });
});
//builder.Services.AddMassTransitHostedService();

// General Configuration
builder.Services.AddScoped<InventorySuccessConsumer>();

#endregion

#region Auto Mapper

// Auto Mapper Config ...

var mapperConfig = new MapperConfiguration(c =>
{
    c.AddProfile(new MapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);

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
