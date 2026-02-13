using System;

namespace Satlink
{
	public interface INotificationService
	{
		void ShowError(string title, string message, Exception? exception = null);
		void ShowWarning(string title, string message);
		void ShowInfo(string title, string message);
	}
}
