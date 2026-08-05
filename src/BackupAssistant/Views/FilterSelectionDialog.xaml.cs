using BackupAssistant.ViewModels;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;

namespace BackupAssistant.Views
{
    /// <summary>
    /// Interaction logic for FilterSelectionDialog.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class FilterSelectionDialog : ContentDialog
    {
        public FilterSelectionDialog(FilterSelectionViewModel viewModel)
        {
            this.ViewModel = viewModel;

            this.InitializeComponent();
        }

        public FilterSelectionViewModel ViewModel { get; }
    }
}
