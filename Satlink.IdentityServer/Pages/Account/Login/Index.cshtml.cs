using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Satlink.IdentityServer.Pages.Account.Login;

public class IndexModel : PageModel
{
    private readonly TestUserStore _users;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;

    public IndexModel(
        TestUserStore users,
        IIdentityServerInteractionService interaction,
        IEventService events)
    {
        _users = users;
        _interaction = interaction;
        _events = events;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
    }

    public IActionResult OnGet(string? returnUrl)
    {
        Input.ReturnUrl = returnUrl ?? string.Empty;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AuthorizationRequest? context = await _interaction.GetAuthorizationContextAsync(Input.ReturnUrl);

        if (!_users.ValidateCredentials(Input.Username, Input.Password))
        {
            await _events.RaiseAsync(new UserLoginFailureEvent(
                Input.Username, "Invalid credentials", clientId: context?.Client.ClientId));

            ErrorMessage = "Usuario o contraseña incorrectos";
            return Page();
        }

        TestUser user = _users.FindByUsername(Input.Username)!;

        await _events.RaiseAsync(new UserLoginSuccessEvent(
            user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId));

        IdentityServerUser isUser = new IdentityServerUser(user.SubjectId)
        {
            DisplayName = user.Username,
            AdditionalClaims = user.Claims
        };

        AuthenticationProperties props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(isUser, props);

        if (context != null || Url.IsLocalUrl(Input.ReturnUrl))
            return Redirect(Input.ReturnUrl);

        return Redirect("~/");
    }
}
