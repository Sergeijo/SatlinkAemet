using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Extensions.Options;

using Satlink.ApiClient;

namespace Satlink
{
    /// <summary>
    /// Application for downloading Aemet data
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs eventArgs)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            string? baseUrl = Configuration["SatlinkApi:BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "http://localhost:5273/";
            }

            serviceCollection.AddHttpClient<IAemetValuesApiClient, AemetValuesApiClient>(client =>
            {
                client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            });

            serviceCollection.AddScoped<IAemetValuesProvider, AemetValuesProvider>();

            var appSettings = new ApplicationSettings
            {
                url = Configuration["AppConfig:url"],
                api_key = Configuration["AppConfig:api_key"]
            };

            serviceCollection.AddSingleton<IOptions<ApplicationSettings>>(Microsoft.Extensions.Options.Options.Create(appSettings));

            serviceCollection.AddTransient(typeof(MainWindow));
        }
    }
}
