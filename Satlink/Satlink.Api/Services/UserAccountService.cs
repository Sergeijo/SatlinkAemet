using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;

namespace Satlink.Api.Services;

/// <summary>
/// Default implementation of <see cref="IUserAccountService"/> backed by <see cref="AemetDbContext"/>.
/// </summary>
public sealed class UserAccountService : IUserAccountService
{
    private readonly AemetDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public UserAccountService(AemetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.UserAccounts.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
