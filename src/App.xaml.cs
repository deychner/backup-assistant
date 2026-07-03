using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        public App()
        {
            this.Services = ConfigureServices();

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static ServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();

            // Services
            _ = services.AddSingleton<ISettingsService, SettingsService>();
            _ = services.AddSingleton<IDialogService, DialogService>();
            _ = services.AddSingleton<ILogService, LogService>();

            // View models
            _ = services.AddTransient<MainWindowViewModel>();

            // Logging
            ConfigureLogging(services);

            return services.BuildServiceProvider();
        }

        private static void ConfigureLogging(ServiceCollection services)
        {
            var logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Anaheim_Electronics", "logs", "backup-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Hour,
                    retainedFileCountLimit: 5,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });
        }
    }
}
