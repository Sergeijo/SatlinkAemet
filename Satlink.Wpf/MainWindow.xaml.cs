using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Options;

using Satlink.ApiClient;

namespace Satlink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INotificationService _notificationService;

        public MainWindow(IAemetValuesProvider aemetValuesProvider, IOptions<ApplicationSettings> appConfig)
        {
            InitializeComponent();

			_notificationService = new NotificationService();

            MarineZonePredictionViewModel VM = new MarineZonePredictionViewModel(aemetValuesProvider, appConfig, _notificationService);
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
                Log.WriteLog($"[MainWindow] - [Window_MouseDown] : {ex.Message}.{ex.StackTrace}");
            }
        }
    }
}
