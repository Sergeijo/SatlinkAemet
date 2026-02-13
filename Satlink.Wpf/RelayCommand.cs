using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Satlink
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        readonly Predicate<T> _canExecute;
        readonly Action<T> _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand<>"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand<>"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            try
            {
                if (execute == null)
                    throw new ArgumentNullException("execute");
                _execute = execute;
                _canExecute = canExecute;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [RelayCommand<T>], en el procedimiento [public RelayCommand(Action<T> execute, Predicate<T> canExecute)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[RelayCommand<T>] - ctor failed: {ex}");
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            try
            {
                return _canExecute == null ? true : _canExecute((T)parameter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [RelayCommand<T>], en el procedimiento [public Boolean CanExecute(Object parameter)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[RelayCommand<T>] - CanExecute failed: {ex}");

                return false;
            }
        }

        public void Execute(Object parameter)
        {
            try
            {
                _execute((T)parameter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [RelayCommand<T>], en el procedimiento [public void Execute(Object parameter)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[RelayCommand<T>] - Execute failed: {ex}");
            }
        }

    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality to other objects by invoking delegates. The default return value for the CanExecute method is 'true'.
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly Func<Boolean> _canExecute;
        readonly Action _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand<>"/> class and the command can always be executed.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand<>"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {
            try
            {
                if (execute == null)
                    throw new ArgumentNullException("execute");
                _execute = execute;
                _canExecute = canExecute;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [and], en el procedimiento [public RelayCommand(Action execute, Func<Boolean> canExecute)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[RelayCommand] - ctor failed: {ex}");
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            try
            {
                return _canExecute == null ? true : _canExecute();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [and], en el procedimiento [public Boolean CanExecute(Object parameter)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[RelayCommand] - CanExecute failed: {ex}");

                return false;
            }
        }

        public void Execute(Object parameter)
        {
            try
            {
                _execute();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [and], en el procedimiento [public void Execute(Object parameter)]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[RelayCommand] - Execute failed: {ex}");
            }
        }

    }
}
