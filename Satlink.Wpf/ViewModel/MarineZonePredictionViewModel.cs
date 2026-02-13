using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Satlink.ApiClient;

namespace Satlink
{
    class MarineZonePredictionViewModel : ObservableObject
    {
        private readonly IAemetValuesProvider _aemetValuesProvider;
        private readonly INotificationService _notificationService;
		private readonly ILogger _logger;

        private ObservableCollection<MarineZonePredictionItem> _ZonePredictionsList = new ObservableCollection<MarineZonePredictionItem>();

        private readonly IOptions<ApplicationSettings> configuration;

        public MarineZonePredictionViewModel(IAemetValuesProvider aemetValuesProvider, IOptions<ApplicationSettings> appConfig, INotificationService notificationService, ILogger logger)
        {
            _aemetValuesProvider = aemetValuesProvider;
            configuration = appConfig;
			_notificationService = notificationService;
			_logger = logger;
        }

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			set
			{
				if (_isBusy == value)
					return;

				_isBusy = value;
				RaisePropertyChanged(nameof(IsBusy));
				CommandManager.InvalidateRequerySuggested();
			}
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
                    RaisePropertyChanged(nameof(Zone));
                }
                catch (Exception ex)
                {
                    _notificationService.ShowError("ATENCIÓN", $"Se ha producido un error en la clase [ZonePredictionViewModel], en la Propiedad [int Zone]. El error es: {ex.Message}.", ex);
					_logger.LogError(ex, "[ZonePredictionViewModel] - Propiedad [int Zone] : {Message}", ex.Message);
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
                    RaisePropertyChanged(nameof(ColumnCollection));
                }
                catch (Exception ex)
                {
                    _notificationService.ShowError("ATENCIÓN", $"Se ha producido un error en la clase [ZonePredictionViewModel], en la Propiedad [ObservableCollection<DataGridColumn> ColumnCollection]. El error es: {ex.Message}.", ex);
					_logger.LogError(ex, "[ZonePredictionViewModel] - Propiedad [ColumnCollection] : {Message}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets or sets the ZonePredictions
        /// </summary>
        public ObservableCollection<MarineZonePredictionItem> ZonePredictions
        {
            get { return _ZonePredictionsList; }
            set
			{
				_ZonePredictionsList = value;
				RaisePropertyChanged(nameof(ZonePredictions));
			}
        }

        /// <summary>
        /// Gets the DownloadCommand
        /// Throw the Download Aemet Maritime Predictions Values
        /// </summary>
        public ICommand DownloadCommand
        {
            get
            {
                return new AsyncRelayCommand(DownloadPredictionsAsync, CanDownloadPredictions);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal bool CanDownloadPredictions()
        {
			return !IsBusy;
        }

        /// <summary>
        ///  Async download of predictions
        /// </summary>
        internal async Task DownloadPredictionsAsync()
        {
            try
            {
                IsBusy = true;
				ZonePredictions = new ObservableCollection<MarineZonePredictionItem>();

                AemetValuesResult result = await _aemetValuesProvider.GetAemetMarineZonePredictionValuesAsync(configuration.Value?.api_key, configuration.Value?.url, Zone).ConfigureAwait(true);

                if (result.Success)
                {
                    if (result.Value != null)
                    {
                        foreach (var zona in result.Value[0].prediccion.zona)
                        {
							ZonePredictions.Add(new MarineZonePredictionItem
                            {
                                Id = zona.id,
                                Nombre = zona.nombre,
                                Texto = zona.texto
                            });
                        }

                        this.ColumnCollection = new ObservableCollection<DataGridColumn>();

						if (ZonePredictions.Count > 0)
                        {
							foreach (var column in ZonePredictions[0].GetMyProperties())
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
                        }

                        RaisePropertyChanged(nameof(ColumnCollection));
                    }
                    else
                    {
                        _notificationService.ShowError("ATENCIÓN", "Se ha producido un error al intentar descargar los datos de la Api de Aemet: No items returned");
						_logger.LogError("[MarineZonePredictionViewModel] - [DownloadPredictions] : No items returned");
                    }
                }
                else
                {
                    _notificationService.ShowError("ATENCIÓN", $"Se ha producido un error al intentar descargar los datos de la Api de Aemet: {result.Error}");
					_logger.LogError("[MarineZonePredictionViewModel] - [DownloadPredictions] : {Error}", result.Error);
                }
            }
            catch (Exception ex)
            {
                _notificationService.ShowError("ATENCIÓN", $"Se ha producido un error en la clase [ZonePredictionViewModel], en la Propiedad [internal void DownloadPredictions()]. El error es: {ex.Message}.", ex);
				_logger.LogError(ex, "[MarineZonePredictionViewModel] - [DownloadPredictions] : {Message}", ex.Message);
            }
			finally
			{
				IsBusy = false;
			}
        }
    }
}
