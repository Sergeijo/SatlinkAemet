using Satlink.Domain.Models;

namespace Satlink.Logic;

public sealed class AuthLoginResult
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required int ExpiresIn { get; init; }

    public required UserAccount User { get; init; }
}
