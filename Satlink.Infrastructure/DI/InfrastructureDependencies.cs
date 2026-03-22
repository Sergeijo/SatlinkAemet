using System;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Http;

using Satlink.Domain.Interfaces;
using Satlink.Logic;
using Satlink.Infrastructure;
using Satlink.Infrastructure.Dapper;
using Satlink.Infrastructure.DbContxt;
using Satlink.Infrastructure.Messaging;
using Satlink.Infrastructure.Messaging.Consumers;
using Satlink.Infrastructure.Services;

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

            // Unit of Work (SQL Server – used by TransactionBehaviour for Request commands).
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Event bus abstraction backed by MassTransit IPublishEndpoint.
            services.AddScoped<IEventBus, MassTransitEventBus>();

            // MassTransit + RabbitMQ with EF Core outbox pattern.
            RegisterMassTransit(services, configuration);

            services.AddHostedService<AemetSqliteDatabaseInitializer>();

            // IHttpContextAccessor is required by UserContext to read JWT claims.
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();
        }

        /// <summary>
        /// Ensures the SQL Server database and all its tables exist (including the
        /// MassTransit outbox tables added in <see cref="AemetDbContext.OnModelCreating"/>).
        /// <para>
        /// Uses <c>EnsureCreatedAsync</c> which is idempotent: it creates the database
        /// and all missing tables on first run and does nothing on subsequent runs.
        /// For schema changes on an existing database use EF Core migrations instead.
        /// </para>
        /// </summary>
        public static async Task InitializeSqlServerAsync(this IServiceProvider services)
        {
            using IServiceScope scope = services.CreateScope();
            AemetDbContext db = scope.ServiceProvider.GetRequiredService<AemetDbContext>();
            await db.Database.EnsureCreatedAsync();
        }

        private static void RegisterMassTransit(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(bus =>
            {
                // ----------------------------------------------------------------
                // EF Core Outbox (SQL Server via AemetDbContext)
                // Stores published messages in the same transaction as business data.
                // A background delivery service polls the outbox and publishes to RabbitMQ.
                // NOTE: Run migrations after adding outbox tables to AemetDbContext:
                //   dotnet ef migrations add AddMassTransitOutbox --project Satlink.Infrastructure
                //   dotnet ef database update --project Satlink.Infrastructure
                // ----------------------------------------------------------------
                bus.AddEntityFrameworkOutbox<AemetDbContext>(outbox =>
                {
                    outbox.UseSqlServer();

                    // Deliver messages from the outbox on the same bus instance.
                    outbox.UseBusOutbox();
                });

                // Register consumers (one queue per consumer, auto-named by convention).
                bus.AddConsumer<RequestCreatedConsumer>();
                bus.AddConsumer<AemetDownloadSavedConsumer>();

                bus.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(
                        configuration["RabbitMQ:Host"] ?? "localhost",
                        configuration["RabbitMQ:VirtualHost"] ?? "/",
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                            h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                        });

                    // Auto-configure endpoints for all registered consumers.
                    cfg.ConfigureEndpoints(ctx);
                });
            });
        }
    }
}