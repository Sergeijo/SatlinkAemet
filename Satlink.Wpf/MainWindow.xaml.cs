using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Satlink.ApiClient;

namespace Satlink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<MainWindow> _logger;
        private readonly ILogger _vmLogger;

        public MainWindow(IAemetValuesProvider aemetValuesProvider, IOptions<ApplicationSettings> appConfig, INotificationService notificationService, ILogger<MainWindow> logger, ILoggerFactory loggerFactory)
        {
            InitializeComponent();
			_notificationService = notificationService;
			_logger = logger;
			_vmLogger = loggerFactory.CreateLogger(typeof(MarineZonePredictionViewModel).FullName ?? nameof(MarineZonePredictionViewModel));

            MarineZonePredictionViewModel VM = new MarineZonePredictionViewModel(aemetValuesProvider, appConfig, _notificationService, _vmLogger);
            this.DataContext = VM;

            string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Window_MouseDown
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                    this.DragMove();
            }
            catch (Exception ex)
            {
                _notificationService.ShowError("ATENCIÓN", $"Se ha producido un error en la clase [MainWindow], en el procedimiento [Window_MouseDown]. El error es: {ex.Message}.", ex);
				_logger.LogError(ex, "[MainWindow] - [Window_MouseDown] : {Message}", ex.Message);
            }
        }
    }
}
