using BackupAssistant.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            this.DataContext = App.Current.Services.GetService<MainWindowViewModel>();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(null);
        }
    }
}
