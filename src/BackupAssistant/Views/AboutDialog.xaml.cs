using BackupAssistant.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;

namespace BackupAssistant.Views
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class AboutDialog : ContentDialog
    {
        public AboutDialog(AboutViewModel viewModel)
        {
            this.ViewModel = viewModel;

            this.InitializeComponent();
        }

        public AboutViewModel ViewModel { get; }
    }
}
