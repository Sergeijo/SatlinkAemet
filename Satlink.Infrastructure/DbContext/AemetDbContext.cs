using Microsoft.EntityFrameworkCore;
using Satlink.Domain.Models;

namespace Satlink.Infrastructure.DI
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
        }
    }
}