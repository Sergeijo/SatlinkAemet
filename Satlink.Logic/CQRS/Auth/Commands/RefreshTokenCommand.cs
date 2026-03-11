using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace Satlink.Logic.CQRS.Auth.Commands;

/// <summary>
/// Command to refresh an access token using a refresh token.
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthRefreshResult>>;

/// <summary>
/// Handler for <see cref="RefreshTokenCommand"/>.
/// </summary>
public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthRefreshResult>>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<AuthRefreshResult>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
        => _authService.RefreshAsync(request.RefreshToken, cancellationToken);
}
