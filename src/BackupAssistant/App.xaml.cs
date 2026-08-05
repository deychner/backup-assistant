using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using BackupAssistant.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;

namespace BackupAssistant
{
    /// <summary>
    /// Application entry point and composition root.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            this.Services = ConfigureServices();

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use.
        /// </summary>
        public static new App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Resolves a registered service.
        /// </summary>
        public static T GetService<T>() where T : class => Current.Services.GetRequiredService<T>();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = GetService<MainWindow>();
            _window.Activate();
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static ServiceProvider ConfigureServices()
        {
            ServiceCollection services = new();

            // Services
            _ = services.AddSingleton<IBackupService, BackupService>();
            _ = services.AddSingleton<IFileSystem, FileSystem>();
            _ = services.AddSingleton<ISettingsService, JsonSettingsService>();
            _ = services.AddSingleton<IApplicationService, ApplicationService>();

            // The dialog service needs the main window in order to parent its dialogs, while the
            // main window needs view models that depend on the dialog service. Handing the dialog
            // service factories instead of instances keeps that from becoming a constructor cycle.
            _ = services.AddSingleton<IDialogService>(provider => new DialogService(
                provider.GetRequiredService<MainWindow>,
                provider.GetRequiredService<AboutViewModel>));

            // Views
            _ = services.AddSingleton<MainWindow>();

            // View models
            _ = services.AddTransient<MainWindowViewModel>();
            _ = services.AddTransient<AboutViewModel>();

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
