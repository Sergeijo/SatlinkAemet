namespace Satlink.Logic;

/// <summary>
/// Provides identity information about the user executing the current request.
/// Populated by <c>RequestLoggingMiddleware</c> after JWT authentication and
/// consumed by <see cref="CQRS.Behaviours.LoggingBehaviour{TRequest,TResponse}"/>.
/// </summary>
public interface IUserContext
{
    /// <summary>Gets the user's unique identifier claim (<c>NameIdentifier</c>), or <c>null</c> for anonymous requests.</summary>
    string? UserId { get; }

    /// <summary>Gets the user's e-mail claim, or <c>null</c> for anonymous requests.</summary>
    string? Email { get; }

    /// <summary>Gets the user's role claim, or <c>null</c> when no role is present.</summary>
    string? Role { get; }

    /// <summary>Gets a value indicating whether the current request is authenticated.</summary>
    bool IsAuthenticated { get; }
}
