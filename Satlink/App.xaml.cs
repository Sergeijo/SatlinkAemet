using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Extensions.Options;

using Satlink.Infrastructure.DI;
using Satlink.Logic.DI;

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

        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            serviceCollection.RegisterInfrastructureDependencies(Configuration);
            serviceCollection.RegisterLogicDependencies();
            //serviceCollection.AddOptions<ApplicationSettings>().Configure<IConfiguration>((settings, configuration) => configuration.GetSection("AppConfig").Bind(settings));
            //serviceCollection.AddSingleton(x => x.GetService<IOptions<ApplicationSettings>>().Value);
            serviceCollection.Configure<ApplicationSettings>(options => Configuration.GetSection("AppConfig").Bind(options));
            serviceCollection.AddTransient(typeof(MainWindow));
        }
    }
}
