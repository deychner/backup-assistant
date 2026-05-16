using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BackupAssistant
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        public Window? m_window;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
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
        /// Invoked when the application is launched.
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

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

            return services.BuildServiceProvider();
        }
    }
}
