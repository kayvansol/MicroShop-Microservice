using Discount.API.Context;
using Discount.API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region DbContext

builder.Services.AddDbContext<MicroShopDiscountContext>(option => option.UseSqlServer(builder.Configuration["ApplicationOptions:StoreConnectionString"]));

builder.Services.AddScoped<MicroShopDiscountContext>();

builder.Services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));

builder.Services.AddScoped<ICouponRepository, CouponRepository>();

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
