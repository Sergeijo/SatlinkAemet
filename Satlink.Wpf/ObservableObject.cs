using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Satlink
{
    /// <summary>
    /// Defines the <see cref="ObservableObject" />
    /// </summary>
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
		private static ILogger? _logger;

		public static void SetLogger(ILogger logger)
		{
			_logger = logger;
		}

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
				if (_logger != null)
					_logger.LogError(ex, "[ObservableObject] - OnPropertyChanged failed: {Message}", ex.Message);
				else
					Trace.TraceError($"[ObservableObject] - OnPropertyChanged failed: {ex}");
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
				if (_logger != null)
					_logger.LogError(ex, "[ObservableObject] - RaisePropertyChanged<T> failed: {Message}", ex.Message);
				else
					Trace.TraceError($"[ObservableObject] - RaisePropertyChanged<T> failed: {ex}");
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
				if (_logger != null)
					_logger.LogError(ex, "[ObservableObject] - RaisePropertyChanged failed: {Message}", ex.Message);
				else
					Trace.TraceError($"[ObservableObject] - RaisePropertyChanged failed: {ex}");
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
				if (_logger != null)
					_logger.LogError(ex, "[ObservableObject] - VerifyPropertyName failed: {Message}", ex.Message);
				else
					Trace.TraceError($"[ObservableObject] - VerifyPropertyName failed: {ex}");
            }
        }
    }
}
