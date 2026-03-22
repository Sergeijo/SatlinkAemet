using Duende.IdentityServer.Models;

using IdentityModel;

namespace Satlink.IdentityServer;

/// <summary>
/// In-memory Identity Server configuration: resources, scopes and clients (mock/dev).
/// </summary>
public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [
        // Declare which user claims must be included in the access token when
        // this scope is requested. Without UserClaims the token only contains 'sub'.
        new ApiScope("satlink-api", "Satlink API")
        {
            UserClaims = { JwtClaimTypes.Email, JwtClaimTypes.Role, JwtClaimTypes.Name }
        }
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("satlink-api", "Satlink API")
        {
            Scopes = { "satlink-api" },
            // Redundant but explicit: also declare at the resource level so Duende IS
            // aggregates claims from both the scope and the resource correctly.
            UserClaims = { JwtClaimTypes.Email, JwtClaimTypes.Role, JwtClaimTypes.Name }
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
        // WPF desktop: Resource Owner Password grant (dev/mock only — deprecated in OAuth 2.1)
        new Client
        {
            ClientId = "satlink-wpf",
            ClientName = "Satlink WPF Desktop",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("satlink-wpf-secret".Sha256()) },
            AllowedScopes = { "openid", "profile", "email", "satlink-api" },
            AccessTokenLifetime = 3600,
            AllowOfflineAccess = true
        },

        // Angular SPA: Authorization Code + PKCE
        new Client
        {
            ClientId = "satlink-angular",
            ClientName = "Satlink Angular SPA",
            AllowedGrantTypes = GrantTypes.Code,
            RequireClientSecret = false,
            RequirePkce = true,
            RequireConsent = false,
            RedirectUris = { "http://localhost:4200/auth/callback" },
            PostLogoutRedirectUris = { "http://localhost:4200" },
            AllowedCorsOrigins = { "http://localhost:4200" },
            AllowedScopes = { "openid", "profile", "email", "satlink-api" },
            AccessTokenLifetime = 3600,
            AllowOfflineAccess = true
        }
    ];
}
