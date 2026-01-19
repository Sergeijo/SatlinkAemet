using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Options;

using Satlink.ApiClient;

namespace Satlink
{
    class MarineZonePredictionViewModel : ObservableObject
    {
        private readonly IAemetValuesProvider _aemetValuesProvider;

        private ObservableCollection<MarineZonePrediction> _ZonePredictionsList;

        private readonly IOptions<ApplicationSettings> configuration;

        public MarineZonePredictionViewModel(IAemetValuesProvider aemetValuesProvider, IOptions<ApplicationSettings> appConfig)
        {
            _aemetValuesProvider = aemetValuesProvider;
            configuration = appConfig;
        }

        /// <summary>
        /// Gets the AllZonesList
        /// </summary>
        public Dictionary<int, string> AllZonesList
        {
            get
            {
                Dictionary<int, string> zones = new Dictionary<int, string>();
                zones.Add(0, "Océano Atlántico al sur de 35º N");
                zones.Add(1, "Océano Atlántico al norte de 30º N");
                zones.Add(2, "Mar Mediterráneo");

                return zones;
            }
        }

        /// <summary>
        /// Defines the _zone
        /// </summary>
        private int _zone = 2;

        /// <summary>
        /// Gets or sets the Zone
        /// </summary>
        public int Zone
        {
            get { return _zone; }
            set
            {
                try
                {
                    _zone = value;
                    RaisePropertyChanged("Zone");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Se ha producido un error en la clase [ZonePredictionViewModel], en la Propiedad [int Zone]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.WriteLog($"[ZonePredictionViewModel] - Propiedad [int Zone] : {ex.Message}.{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Defines the _columnCollection
        /// </summary>
        private ObservableCollection<DataGridColumn> _columnCollection = new ObservableCollection<DataGridColumn>();

        /// <summary>
        /// Gets or sets the ColumnCollection
        /// </summary>
        public ObservableCollection<DataGridColumn> ColumnCollection
        {
            get
            {
                return this._columnCollection;
            }
            set
            {
                try
                {
                    _columnCollection = value;
                    RaisePropertyChanged("ColumnCollection");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Se ha producido un error en la clase [ZonePredictionViewModel], en la Propiedad [ObservableCollection<DataGridColumn> ColumnCollection]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.WriteLog($"[ZonePredictionViewModel] - Propiedad [ObservableCollection<DataGridColumn> ColumnCollection] : {ex.Message}.{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Gets or sets the ZonePredictions
        /// </summary>
        public ObservableCollection<MarineZonePrediction> ZonePredictions
        {
            get { return _ZonePredictionsList; }
            set { _ZonePredictionsList = value; }
        }

        /// <summary>
        /// Gets the DownloadCommand
        /// Throw the Download Aemet Maritime Predictions Values
        /// </summary>
        public ICommand DownloadCommand
        {
            get
            {
                return new RelayCommand(DownloadPredictions, CanDownloadPredictions);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool CanDownloadPredictions()
        {
            return true;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <returns></returns>
        internal void DownloadPredictions()
        {
            try
            {
                _ZonePredictionsList = new ObservableCollection<MarineZonePrediction>();

                AemetValuesResult result = _aemetValuesProvider.GetAemetMarineZonePredictionValues(configuration.Value?.api_key, configuration.Value?.url, Zone);

                if (result.Success)
                {
                    result.Value?.FirstOrDefault().prediccion.zona.ForEach(zona => _ZonePredictionsList.Add(new MarineZonePrediction()
                    {
                        Id = zona.id,
                        Texto = zona.texto,
                        Nombre = zona.nombre
                    }));

                    this.ColumnCollection = new ObservableCollection<DataGridColumn>();

                    foreach (var column in _ZonePredictionsList[0].GetMyProperties())
                    {
                        ColumnCollection.Add(new DataGridTextColumn()
                        {
                            Header = column,
                            Binding = new System.Windows.Data.Binding(column)
                            {
                                Mode = System.Windows.Data.BindingMode.TwoWay,
                                ValidatesOnExceptions = true,
                                UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.LostFocus,
                                NotifyOnTargetUpdated = true
                            },
                            //Width = 80,
                            IsReadOnly = true
                        });
                    }

                    RaisePropertyChanged("ColumnCollection");
                    RaisePropertyChanged("ZonePredictions");
                }
                else
                {
                    MessageBox.Show($"Se ha producido un error al intentar descargar los datos de la Api de Aemet: {result.Error}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                    Log.WriteLog($"[MarineZonePredictionViewModel] - [DownloadPredictions] : Se ha producido un error al intentar descargar los datos de la Api de Aemet: {result.Error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se ha producido un error en la clase [ZonePredictionViewModel], en la Propiedad [internal void DownloadPredictions()]. El error es: {ex.Message}. {ex.InnerException?.ToString()}", "ATENCIÓN", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.WriteLog($"[MarineZonePredictionViewModel] - [DownloadPredictions] : {ex.Message}.{ex.StackTrace}");
            }
        }
    }
}
