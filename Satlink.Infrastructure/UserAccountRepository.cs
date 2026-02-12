using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Satlink.Domain.Models;
using Satlink.Infrastructure.DI;
using Satlink.Logic;

namespace Satlink.Infrastructure;

internal sealed class UserAccountRepository : IUserAccountRepository
{
    private readonly AemetDbContext _dbContext;

    public UserAccountRepository(AemetDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _dbContext.UserAccounts
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
