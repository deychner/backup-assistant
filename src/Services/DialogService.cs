using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace BackupAssistant.Services
{
    [ExcludeFromCodeCoverage]
    public class DialogService : IDialogService
    {
        public (bool?, string) ShowOpenFolderDialog(string selectedPath = "")
        {
            OpenFolderDialog dialog = new()
            {
                AddToRecent = false,
                InitialDirectory = selectedPath,
                Multiselect = false,
                ShowHiddenItems = false,
            };

            return (dialog.ShowDialog(), dialog.FolderName);
        }

        public bool? ShowDialog<T>(object viewModel) where T : Window, new()
        {
            T t = new()
            {
                DataContext = viewModel
            };

            return t.ShowDialog();
        }
    }
}
