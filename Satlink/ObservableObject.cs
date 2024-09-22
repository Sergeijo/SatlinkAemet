using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Satlink
{
    /// <summary>
    /// Defines the <see cref="ObservableObject" />
    /// </summary>
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Defines the PropertyChanged
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The OnPropertyChanged
        /// </summary>
        /// <param name="e">The e<see cref="PropertyChangedEventArgs"/></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            try
            {
                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [ObservableObject], en el procedimiento [protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[ObservableObject] - [protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// The RaisePropertyChanged
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpresssion">The propertyExpresssion<see cref="Expression{Func{T}}"/></param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            try
            {
                var propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
                this.RaisePropertyChanged(propertyName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [ObservableObject], en el procedimiento [protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[ObservableObject] - [protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// The RaisePropertyChanged
        /// </summary>
        /// <param name="propertyName">The propertyName<see cref="String"/></param>
        protected void RaisePropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    VerifyPropertyName(propertyName);
                    OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [ObservableObject], en el procedimiento [protected void RaisePropertyChanged([CallerMemberName] String propertyName = null)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[ObservableObject] - [protected void RaisePropertyChanged([CallerMemberName] String propertyName = null)] : {ex.Message}.{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Warns the developer if this Object does not have a public property with
        /// the specified name. This method does not exist in a Release build.
        /// </summary>
        /// <param name="propertyName">The propertyName<see cref="String"/></param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(String propertyName)
        {
            try
            {
                // verify that the property name matches a real,  
                // public, instance property on this Object.
                if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                {
                    Debug.Fail("Invalid property name: " + propertyName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [ObservableObject], en el procedimiento [public void VerifyPropertyName(String propertyName)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[ObservableObject] - [public void VerifyPropertyName(String propertyName)] : {ex.Message}.{ex.StackTrace}");
            }
        }
    }
}
