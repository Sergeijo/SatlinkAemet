using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Satlink.Infrastructure;
using Satlink.Infrastructure.Dapper;
using Satlink.Infrastructure.DbContxt;
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

            // Dapper read side (CQRS queries)
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IRequestsQueryRepository, RequestsDapperQueryRepository>();

            services.AddHttpClient<IAemetOpenDataClient, AemetOpenDataClient>();
            services.AddScoped<IAemetRepository, AemetRepository>();
            services.AddScoped<IAemetJsonSerializer, AemetJsonSerializer>();
            services.AddScoped<IRequestsRepository, RequestsRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        }
    }
}