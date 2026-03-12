using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace Satlink.Logic.CQRS.Auth.Commands;

/// <summary>
/// Command to authenticate a user.
/// </summary>
public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthLoginResult>>;

/// <summary>
/// Handler for <see cref="LoginCommand"/>.
/// </summary>
public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<AuthLoginResult>>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<AuthLoginResult>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
        => _authService.LoginAsync(request.Email, request.Password, cancellationToken);
}
