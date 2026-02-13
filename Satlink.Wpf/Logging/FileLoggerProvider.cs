using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Satlink
{
	public sealed class FileLoggerProvider : ILoggerProvider
	{
		private readonly string _logsDirectory;
		private readonly LogLevel _minLevel;
		private readonly object _lock = new();

		public FileLoggerProvider(string logsDirectory, LogLevel minLevel = LogLevel.Information)
		{
			_logsDirectory = logsDirectory;
			_minLevel = minLevel;
		}

		public ILogger CreateLogger(string categoryName)
			=> new FileLogger(categoryName, _logsDirectory, _minLevel, _lock);

		public void Dispose() { }

		private sealed class FileLogger : ILogger
		{
			private readonly string _categoryName;
			private readonly string _logsDirectory;
			private readonly LogLevel _minLevel;
			private readonly object _lock;

			public FileLogger(string categoryName, string logsDirectory, LogLevel minLevel, object @lock)
			{
				_categoryName = categoryName;
				_logsDirectory = logsDirectory;
				_minLevel = minLevel;
				_lock = @lock;
			}

			public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

			public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
				Func<TState, Exception?, string> formatter)
			{
				if (!IsEnabled(logLevel))
					return;

				try
				{
					Directory.CreateDirectory(_logsDirectory);
					var filePath = Path.Combine(_logsDirectory, $"satlink_{DateTime.Now:yyyyMMdd}.log");

					var line = $"{DateTimeOffset.Now:O} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
					if (exception != null)
						line += Environment.NewLine + exception;

					lock (_lock)
					{
						File.AppendAllText(filePath, line + Environment.NewLine);
					}
				}
				catch
				{
					// swallow logging exceptions
				}
			}

			private sealed class NullScope : IDisposable
			{
				public static readonly NullScope Instance = new();
				public void Dispose() { }
			}
		}
	}
}
