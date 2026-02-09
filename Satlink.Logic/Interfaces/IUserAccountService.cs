using System.Threading;
using System.Threading.Tasks;

using Satlink.Domain.Models;

namespace Satlink.Logic;

/// <summary>
/// Provides user account lookup operations for authentication.
/// </summary>
public interface IUserAccountService
{
    /// <summary>
    /// Gets a user account by email.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user account, or <see langword="null"/> if not found.</returns>
    Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}
