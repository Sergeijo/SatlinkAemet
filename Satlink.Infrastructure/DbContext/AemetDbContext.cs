using Microsoft.EntityFrameworkCore;
using Satlink.Domain.Models;

namespace Satlink.Infrastructure.DI
{
    public class AemetDbContext : DbContext
    {
        public AemetDbContext(DbContextOptions<AemetDbContext> options) : base(options)
        {
        }

        public DbSet<Request> zonePredictionsItems { get; set; }
    }
}