using System.Security.Claims;

using Duende.IdentityServer.Test;

using IdentityModel;

namespace Satlink.IdentityServer;

/// <summary>
/// In-memory mock users for development and testing.
/// </summary>
public static class MockUsers
{
    public static List<TestUser> Users =>
    [
        new TestUser
        {
            SubjectId = "user-1",
            Username = "alice",
            Password = "alice123",
            IsActive = true,
            Claims =
            [
                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                new Claim(JwtClaimTypes.Email, "alice@satlink.dev"),
                new Claim(JwtClaimTypes.Role, "Admin")
            ]
        },
        new TestUser
        {
            SubjectId = "user-2",
            Username = "bob",
            Password = "bob123",
            IsActive = true,
            Claims =
            [
                new Claim(JwtClaimTypes.Name, "Bob Jones"),
                new Claim(JwtClaimTypes.Email, "bob@satlink.dev"),
                new Claim(JwtClaimTypes.Role, "User")
            ]
        }
    ];
}
