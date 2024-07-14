using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Resources.Services;

namespace UserService.Infrastructure.Resources
{
    public static class ServiceRegistration
    {
        public static void AddResourcesInfrastructure(this IServiceCollection services)
        {
            services.AddSingleton<ITranslator, Translator>();
        }
    }
}
