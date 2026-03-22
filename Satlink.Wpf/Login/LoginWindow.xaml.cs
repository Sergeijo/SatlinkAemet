using System.Windows;
using System.Windows.Input;

namespace Satlink.Login;

/// <summary>
/// Code-behind for <see cref="LoginWindow"/>.
/// Reads the PasswordBox value (which cannot be data-bound for security reasons)
/// and passes it to <see cref="LoginViewModel.LoginAsync"/>.
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();

        _viewModel          = viewModel;
        DataContext         = _viewModel;

        // When the ViewModel signals success, close this dialog with result = true
        // so that App.OnStartup knows it can open MainWindow.
        _viewModel.LoginSucceeded += () =>
        {
            DialogResult = true;
            Close();
        };

        UsernameBox.Focus();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.IsBusy) return;
        await _viewModel.LoginAsync(PasswordBox.Password);
    }

    // Allow pressing Enter from either input field to trigger login.
    private async void Input_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            await _viewModel.LoginAsync(PasswordBox.Password);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            try { DragMove(); }
            catch { /* can happen if button click races with drag */ }
        }
    }
}
