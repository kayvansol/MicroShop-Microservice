using MicroShop.OrderApi.Rest.Startup;
using MicroShop.Application;
using MicroShop.Infra.Sql.Extensions;
using System.Configuration;
using System.Reflection;
using Microsoft.AspNetCore.Server.Kestrel.Core;

#region Services

var builder = WebApplication.CreateBuilder(args);

#region Kestrel Config

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5180, o =>
    {
        o.Protocols = HttpProtocols.Http1AndHttp2;   // گارانتی قطعی
        //o.UseHttps();
    });
});

#endregion

builder.Services.Register(builder.Configuration);

builder.Services.AddInfraServicesRegister();

builder.Services.AddApplicationServicesRegister();

builder.Services.AddMessageBus(builder.Configuration, Assembly.GetExecutingAssembly());

builder.Host.Register(builder.Configuration);

#endregion

#region Application

var app = builder.Build();

app.Register();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

//app.MapControllers().RequireAuthorization("MyApiPolicy");

app.MapControllers();

app.Run(); 

#endregion

