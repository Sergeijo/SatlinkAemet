using Microsoft.Extensions.DependencyInjection;

namespace Satlink.Logic.DI
{
    public static class LogicDependencies
    {
        public static void RegisterLogicDependencies(this IServiceCollection services)
        {
            services.AddScoped<IAemetValuesService, AemetValuesService>();
            services.AddScoped<IRequestsService, RequestsService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}