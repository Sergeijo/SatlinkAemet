using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Satlink.Logic;

namespace Satlink.Infrastructure.DI
{
    public static class InfrastructureDependencies
    {
        public static void RegisterInfrastructureDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AemetDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SatlinkApp")));
            services.AddScoped<IAemetRepository, AemetRepository>();
        }
    }
}