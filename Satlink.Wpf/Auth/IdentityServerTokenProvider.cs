using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using IdentityModel.Client;

namespace Satlink.Auth;

/// <summary>
/// Obtains and caches access tokens from Duende Identity Server via the Resource Owner
/// Password Credentials grant. Credentials are supplied at login time, not at
/// construction, so the object can be registered as a singleton before the user logs in.
/// </summary>
public sealed class IdentityServerTokenProvider : ITokenProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _authority;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _scope;

    // Set after a successful LoginAsync call; used to silently refresh expired tokens.
    private string? _username;
    private string? _password;

    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public IdentityServerTokenProvider(
        IHttpClientFactory httpClientFactory,
        string authority,
        string clientId,
        string clientSecret,
        string scope)
    {
        _httpClientFactory = httpClientFactory;
        _authority = authority;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _scope = scope;
    }

    /// <inheritdoc/>
    public bool IsAuthenticated =>
        _cachedToken is not null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-1);

    /// <inheritdoc/>
    public async Task<(bool Success, string? Error)> LoginAsync(
        string username, string password, CancellationToken cancellationToken = default)
    {
        (string? token, DateTime expiry, string? error) =
            await RequestTokenAsync(username, password, cancellationToken);

        if (error is not null)
            return (false, error);

        _username    = username;
        _password    = password;
        _cachedToken = token;
        _tokenExpiry = expiry;
        return (true, null);
    }

    /// <inheritdoc/>
    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (IsAuthenticated)
            return _cachedToken;

        // No credentials yet — caller must invoke LoginAsync first.
        if (_username is null || _password is null)
            return null;

        // Token expired but credentials are available → silent refresh.
        (string? token, DateTime expiry, _) =
            await RequestTokenAsync(_username, _password, cancellationToken);

        _cachedToken = token;
        _tokenExpiry = expiry;
        return _cachedToken;
    }

    // -------------------------------------------------------------------------

    private async Task<(string? Token, DateTime Expiry, string? Error)> RequestTokenAsync(
        string username, string password, CancellationToken cancellationToken)
    {
        HttpClient client = _httpClientFactory.CreateClient("IdentityServer");

        DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(
            new DiscoveryDocumentRequest
            {
                Address = _authority,
                Policy = { RequireHttps = false }  // dev/mock only
            }, cancellationToken);

        if (disco.IsError)
            return (null, DateTime.MinValue,
                $"No se pudo obtener el discovery document: {disco.Error}");

        TokenResponse response = await client.RequestPasswordTokenAsync(
            new PasswordTokenRequest
            {
                Address    = disco.TokenEndpoint,
                ClientId     = _clientId,
                ClientSecret = _clientSecret,
                UserName     = username,
                Password     = password,
                Scope        = _scope
            }, cancellationToken);

        if (response.IsError)
        {
            string error = response.ErrorDescription ?? response.Error
                ?? "Credenciales incorrectos.";
            return (null, DateTime.MinValue, error);
        }

        return (response.AccessToken,
                DateTime.UtcNow.AddSeconds(response.ExpiresIn),
                null);
    }
}
