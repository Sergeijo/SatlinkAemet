using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Satlink.Auth;

/// <summary>
/// Delegating handler that attaches the Identity Server Bearer token to every
/// outgoing request made by the Satlink API HTTP client.
/// </summary>
public sealed class AuthTokenDelegatingHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthTokenDelegatingHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? token = await _tokenProvider.GetAccessTokenAsync(cancellationToken);

        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
