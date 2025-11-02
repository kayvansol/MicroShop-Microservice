using Microsoft.Extensions.DependencyInjection;
using MicroShop.Application.Behaviors;
using System.Reflection;

namespace MicroShop.Application
{
    public static class InjectApplication
    {
        public static void AddApplicationServicesRegister(this IServiceCollection service)
        {

            service.AddMediatR(Assembly.GetExecutingAssembly());

            service.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            service.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        }
    }
}
