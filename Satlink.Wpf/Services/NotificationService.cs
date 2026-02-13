using System;
using System.Windows;

namespace Satlink
{
	public sealed class NotificationService : INotificationService
	{
		public void ShowError(string title, string message, Exception? exception = null)
			=> Show(title, message, exception, NotificationType.Error);

		public void ShowWarning(string title, string message)
			=> Show(title, message, null, NotificationType.Warning);

		public void ShowInfo(string title, string message)
			=> Show(title, message, null, NotificationType.Info);

		private static void Show(string title, string message, Exception? exception, NotificationType type)
		{
			try
			{
				Application.Current?.Dispatcher?.BeginInvoke(() =>
				{
					var window = new NotificationWindow(title, message, exception?.ToString(), type)
					{
						Owner = Application.Current?.MainWindow
					};

					window.Show();
				});
			}
			catch
			{
				// Fallback (should be rare)
				MessageBox.Show(message, title, MessageBoxButton.OK, type switch
				{
					NotificationType.Error => MessageBoxImage.Error,
					NotificationType.Warning => MessageBoxImage.Warning,
					_ => MessageBoxImage.Information
				});
			}
		}

		private enum NotificationType
		{
			Info,
			Warning,
			Error
		}

		private sealed class NotificationWindow : Window
		{
			public NotificationWindow(string title, string message, string? details, NotificationType type)
			{
				Title = title;
				WindowStyle = WindowStyle.ToolWindow;
				ResizeMode = ResizeMode.NoResize;
				ShowInTaskbar = false;
				Topmost = true;
				SizeToContent = SizeToContent.WidthAndHeight;
				WindowStartupLocation = WindowStartupLocation.CenterOwner;

				var (bg, fg) = type switch
				{
					NotificationType.Error => (new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(254, 242, 242)), System.Windows.Media.Brushes.DarkRed),
					NotificationType.Warning => (new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 251, 235)), System.Windows.Media.Brushes.DarkGoldenrod),
					_ => (new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(239, 246, 255)), System.Windows.Media.Brushes.DarkBlue)
				};

				Background = bg;

				var root = new System.Windows.Controls.StackPanel
				{
					Margin = new Thickness(14),
					MaxWidth = 520
				};

				root.Children.Add(new System.Windows.Controls.TextBlock
				{
					Text = message,
					Foreground = fg,
					TextWrapping = TextWrapping.Wrap,
					FontWeight = FontWeights.SemiBold,
					Margin = new Thickness(0, 0, 0, 10)
				});

				if (!string.IsNullOrWhiteSpace(details))
				{
					var expander = new System.Windows.Controls.Expander
					{
						Header = "Details",
						IsExpanded = false,
						Margin = new Thickness(0, 0, 0, 10)
					};

					expander.Content = new System.Windows.Controls.TextBox
					{
						Text = details,
						IsReadOnly = true,
						TextWrapping = TextWrapping.Wrap,
						VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
						HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
						Height = 140
					};

					root.Children.Add(expander);
				}

				var buttons = new System.Windows.Controls.StackPanel
				{
					Orientation = System.Windows.Controls.Orientation.Horizontal,
					HorizontalAlignment = HorizontalAlignment.Right
				};

				var ok = new System.Windows.Controls.Button
				{
					Content = "OK",
					MinWidth = 80,
					IsDefault = true
				};
				ok.Click += (_, _) => Close();

				buttons.Children.Add(ok);
				root.Children.Add(buttons);

				Content = root;
			}
		}
	}
}
