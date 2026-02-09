using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;
using Satlink.Logic;

namespace Satlink.Api.Services;

/// <summary>
/// Default implementation of <see cref="IUserAccountService"/> backed by <see cref="IUserAccountRepository"/>.
/// </summary>
public sealed class UserAccountService : IUserAccountService
{
    private readonly IUserAccountRepository _userAccountRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountService"/> class.
    /// </summary>
    /// <param name="userAccountRepository">The user account repository.</param>
    public UserAccountService(IUserAccountRepository userAccountRepository)
    {
        _userAccountRepository = userAccountRepository;
    }

    /// <inheritdoc />
    public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _userAccountRepository.GetByEmailAsync(email, cancellationToken);
    }
}
