namespace Satlink.Logic;

public sealed class AuthRefreshResult
{
    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required int ExpiresIn { get; init; }
}
