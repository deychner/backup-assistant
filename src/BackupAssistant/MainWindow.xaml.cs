using BackupAssistant.ViewModels;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;
using Windows.Graphics;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class MainWindow : Window
    {
        private const int InitialClientWidth = 560;
        private const int InitialClientHeight = 500;

        public MainWindow(MainWindowViewModel viewModel)
        {
            this.ViewModel = viewModel;

            this.InitializeComponent();

            this.AppWindow.SetIcon("assets/icon.ico");

            // The title bar defaults to TitleBarTheme.Legacy, which stays light even when the app
            // is dark. UseDefaultAppMode makes the caption follow the Windows app theme, so the
            // whole window switches between light and dark together.
            this.AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

            SizeAndCenter();
        }

        /// <summary>
        /// Gets the view model backing this window. Public and strongly typed so that the
        /// XAML can reach it through compiled <c>x:Bind</c> expressions.
        /// </summary>
        public MainWindowViewModel ViewModel { get; }

        /// <summary>
        /// WinUI has no equivalent of WPF's SizeToContent, so the window is given an explicit
        /// starting size and centred on the display it opened on.
        /// </summary>
        private void SizeAndCenter()
        {
            this.AppWindow.ResizeClient(new SizeInt32(InitialClientWidth, InitialClientHeight));

            DisplayArea display = DisplayArea.GetFromWindowId(this.AppWindow.Id, DisplayAreaFallback.Nearest);

            this.AppWindow.Move(new PointInt32(
                display.WorkArea.X + ((display.WorkArea.Width - this.AppWindow.Size.Width) / 2),
                display.WorkArea.Y + ((display.WorkArea.Height - this.AppWindow.Size.Height) / 2)));
        }
    }
}
