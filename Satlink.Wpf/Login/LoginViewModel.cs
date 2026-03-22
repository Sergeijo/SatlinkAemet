using System;
using System.Threading.Tasks;

using Satlink.Auth;

namespace Satlink.Login;

/// <summary>
/// ViewModel for the login window. Delegates the actual token request to
/// <see cref="ITokenProvider.LoginAsync"/> and exposes bindable state for
/// the view (username, error message, busy flag).
/// </summary>
public sealed class LoginViewModel : ObservableObject
{
    private readonly ITokenProvider _tokenProvider;

    private string _username    = string.Empty;
    private string _errorMessage = string.Empty;
    private bool   _isBusy;

    /// <summary>Fired on the UI thread when authentication succeeds.</summary>
    public event Action? LoginSucceeded;

    public LoginViewModel(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            RaisePropertyChanged();
            // Clear stale error when the user starts typing again.
            if (!string.IsNullOrEmpty(_errorMessage))
                ErrorMessage = string.Empty;
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set { _errorMessage = value; RaisePropertyChanged(); }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; RaisePropertyChanged(); }
    }

    /// <summary>
    /// Called by the code-behind with the PasswordBox value (which cannot be data-bound).
    /// </summary>
    public async Task LoginAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorMessage = "Introduce usuario y contraseña.";
            return;
        }

        IsBusy       = true;
        ErrorMessage = string.Empty;

        try
        {
            (bool success, string? error) =
                await _tokenProvider.LoginAsync(Username.Trim(), password);

            if (success)
                LoginSucceeded?.Invoke();
            else
                ErrorMessage = error ?? "Credenciales incorrectos.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
