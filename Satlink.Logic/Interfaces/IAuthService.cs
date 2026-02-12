using System.Threading;
using System.Threading.Tasks;

namespace Satlink.Logic;

/// <summary>
/// Authentication use case.
/// </summary>
public interface IAuthService
{
    Task<Result<AuthLoginResult>> LoginAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result<AuthRefreshResult>> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
}
