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

            services.AddDbContext<AemetSqliteDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("AemetDownloads")
                    ?? "Data Source=aemet_downloads.db"));

            // Dapper read side (CQRS queries)
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IRequestsQueryRepository, RequestsDapperQueryRepository>();
            services.AddSingleton<ISqliteConnectionFactory, SqliteConnectionFactory>();
            services.AddScoped<IAemetDownloadQueryRepository, AemetDownloadDapperQueryRepository>();

            services.AddHttpClient<IAemetOpenDataClient, AemetOpenDataClient>();
            services.AddScoped<IAemetRepository, AemetRepository>();
            services.AddScoped<IAemetJsonSerializer, AemetJsonSerializer>();
            services.AddScoped<IRequestsRepository, RequestsRepository>();
            services.AddKeyedScoped<IRequestsRepository, AemetSqliteRequestRepository>("Sqlite");
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            services.AddHostedService<AemetSqliteDatabaseInitializer>();
        }
    }
}