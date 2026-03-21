using System.Collections.Generic;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

using Satlink.Domain.Models;

namespace Satlink.Infrastructure.DbContxt
{
    public class AemetDbContext : DbContext
    {
        public AemetDbContext(DbContextOptions<AemetDbContext> options) : base(options)
        {
        }

        public DbSet<PersistedRequest> zonePredictionsItems { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAccount>()
                .HasMany(x => x.RefreshTokens)
                .WithOne(x => x.UserAccount)
                .HasForeignKey(x => x.UserAccountId);

            modelBuilder.Entity<UserAccount>()
                .HasIndex(x => x.Email)
                .IsUnique();

            // Configure PersistedRequest owned types so EF Core treats Origen,
            // Situacion and Prediccion as inline columns in the same table instead
            // of trying to create separate entity tables that require a primary key.
            // MassTransit's AddEntityFrameworkOutbox triggers eager model validation,
            // which surfaces this requirement even if SaveChanges was never called.
            ValueConverter<List<Zona>, string> zonaListConverter = new(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<List<Zona>>(v) ?? new List<Zona>());

            modelBuilder.Entity<PersistedRequest>(entity =>
            {
                entity.HasKey(e => e.id);

                entity.OwnsOne(e => e.origen);
                entity.OwnsOne(e => e.situacion);
                entity.OwnsOne(e => e.prediccion, p =>
                {
                    // Store the zones list as a JSON string – avoids a separate table.
                    p.Property(x => x.zona).HasConversion(zonaListConverter);
                });
            });

            // MassTransit EF Core outbox tables.
            // After adding this, run: dotnet ef migrations add AddMassTransitOutbox
            //                         dotnet ef database update
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}