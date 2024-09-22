using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Satlink.GeneralControls.Header
{

    /// <summary>
    /// Lógica de interacción para Header_Control.xaml
    /// </summary>
    public partial class Header_Control : UserControl
    {
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public String Title
        {
            get { return (String)this.GetValue(TitleProperty); }
            set
            {
                try
                {
                    this.SetValue(TitleProperty, value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en la Propiedad [String Title]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.WriteLog($"[Header_Control] - Propiedad [String Title] : {ex.Message}.{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Defines the TitleProperty
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(
              "Title", typeof(String), typeof(Header_Control), new PropertyMetadata("Bienvenido a Satlink Aemet"));

        /// <summary>
        /// Initializes a new instance of the <see cref="Header_Control"/> class.
        /// </summary>
        public Header_Control()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [Header_Control]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [Header_Control] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// The UserControl_Loaded
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.titleHeader.Content = Title;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [UserControl_Loaded]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [UserControl_Loaded] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Control de titleBar que permite mover la ventana de la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                (App.Current.MainWindow as MainWindow).DragMove();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [titleBar_MouseLeftButtonDown]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [titleBar_MouseLeftButtonDown] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Control de boton que al clicarlo cierra la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (App.Current.MainWindow as MainWindow).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [closeButton_Click]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [closeButton_Click] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Control de boton que al clicarlo minimiza la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (App.Current.MainWindow as MainWindow).WindowState = System.Windows.WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [minButton_Click]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [minButton_Click] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Control de boton que al clicarlo maximiza o pone a tamaño normal la aplicación
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((App.Current.MainWindow as MainWindow).WindowState == System.Windows.WindowState.Normal)
                {
                    (App.Current.MainWindow as MainWindow).WindowState = WindowState.Maximized;
                }
                else
                {
                    (App.Current.MainWindow as MainWindow).WindowState = System.Windows.WindowState.Normal;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [MaxButton_Click]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [MaxButton_Click] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Abre el manual pdf de la heramienta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string pdfPath = "";

                if (pdfPath != null)
                {
                    if (System.IO.File.Exists(pdfPath))
                    {
                        System.Diagnostics.Process.Start(pdfPath);
                    }
                    else
                    {
                        MessageBox.Show($"No se encuentra el archivo de ayuda en la ruta: {pdfPath}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("No se encuentra el archivo de ayuda.", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [Header_Control], en el procedimiento [helpButton_Click]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[Header_Control] - [helpButton_Click] : {ex.Message}.{ex.StackTrace}");
            }
        }
    }
}
