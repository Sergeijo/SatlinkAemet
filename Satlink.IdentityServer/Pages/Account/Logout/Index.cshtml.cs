using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Satlink.IdentityServer.Pages.Account.Logout;

public class IndexModel : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;

    public IndexModel(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    public async Task<IActionResult> OnGetAsync(string? logoutId)
    {
        LogoutRequest logout = await _interaction.GetLogoutContextAsync(logoutId);

        await HttpContext.SignOutAsync();

        if (!string.IsNullOrEmpty(logout?.PostLogoutRedirectUri))
            return Redirect(logout.PostLogoutRedirectUri);

        return Redirect("~/");
    }
}
