using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Satlink
{
    /// <summary>
    /// Defines the <see cref="DataGridColumnsBehavior" />
    /// </summary>
    public class DataGridColumnsBehavior
    {
        /// <summary>
        /// Defines the BindableColumnsProperty
        /// </summary>
        public static readonly DependencyProperty BindableColumnsProperty =
               DependencyProperty.RegisterAttached("BindableColumns",
                                                   typeof(ObservableCollection<DataGridColumn>),
                                                   typeof(DataGridColumnsBehavior),
                                                   new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        /// <summary>
        /// The BindableColumnsPropertyChanged
        /// </summary>
        /// <param name="source">The source<see cref="DependencyObject"/></param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/></param>
        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                DataGrid dataGrid = source as DataGrid;
                ObservableCollection<DataGridColumn> columns = e.NewValue as ObservableCollection<DataGridColumn>;
                dataGrid.Columns.Clear();
                if (columns == null)
                {
                    return;
                }
                foreach (DataGridColumn column in columns)
                {
                    try
                    {
                        dataGrid.Columns.Add(column);
                    }
                    catch (Exception ex)
                    {
                        var traza = dataGrid.Columns.Count();
                    }
                }
                columns.CollectionChanged += (sender, e2) =>
                {
                    NotifyCollectionChangedEventArgs ne = e2 as NotifyCollectionChangedEventArgs;
                    if (ne.Action == NotifyCollectionChangedAction.Reset)
                    {
                        dataGrid.Columns.Clear();
                        if (ne.NewItems != null)
                        {
                            foreach (DataGridColumn column in ne.NewItems)
                            {
                                dataGrid.Columns.Add(column);
                            }
                        }
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Add)
                    {
                        if (ne.NewItems != null)
                        {
                            foreach (DataGridColumn column in ne.NewItems)
                            {
                                dataGrid.Columns.Add(column);
                            }
                        }
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Move)
                    {
                        dataGrid.Columns.Move(ne.OldStartingIndex, ne.NewStartingIndex);
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Remove)
                    {
                        if (ne.OldItems != null)
                        {
                            foreach (DataGridColumn column in ne.OldItems)
                            {
                                dataGrid.Columns.Remove(column);
                            }
                        }
                    }
                    else if (ne.Action == NotifyCollectionChangedAction.Replace)
                    {
                        dataGrid.Columns[ne.NewStartingIndex] = ne.NewItems[0] as DataGridColumn;
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [DataGridColumnsBehavior], en el procedimiento [BindableColumnsPropertyChanged]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[DataGridColumnsBehavior] - BindableColumnsPropertyChanged failed: {ex}");
            }
        }

        /// <summary>
        /// The SetBindableColumns
        /// </summary>
        /// <param name="element">The element<see cref="DependencyObject"/></param>
        /// <param name="value">The value<see cref="ObservableCollection{DataGridColumn}"/></param>
        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumn> value)
        {
            try
            {
                element.SetValue(BindableColumnsProperty, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [DataGridColumnsBehavior], en el procedimiento [SetBindableColumns]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[DataGridColumnsBehavior] - SetBindableColumns failed: {ex}");
            }
        }

        /// <summary>
        /// The GetBindableColumns
        /// </summary>
        /// <param name="element">The element<see cref="DependencyObject"/></param>
        /// <returns>The <see cref="ObservableCollection{DataGridColumn}"/></returns>
        public static ObservableCollection<DataGridColumn> GetBindableColumns(DependencyObject element)
        {
            try
            {
                return (ObservableCollection<DataGridColumn>)element.GetValue(BindableColumnsProperty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [DataGridColumnsBehavior], en el procedimiento [GetBindableColumns]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
				Trace.TraceError($"[DataGridColumnsBehavior] - GetBindableColumns failed: {ex}");
                return null;
            }
        }
    }
}
