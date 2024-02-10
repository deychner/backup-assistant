using BackupAssistant.ViewModels;
using Microsoft.Win32;
using System.Windows;

namespace BackupAssistant.Services
{
    public class DialogService : IDialogService
    {
        public (bool?, string) ShowFolderBrowserDialog(string selectedPath = "")
        {
            OpenFolderDialog dialog = new()
            {
                InitialDirectory = selectedPath,
                Multiselect = false
            };

            return (dialog.ShowDialog(), dialog.FolderName);
        }

        public bool? ShowDialog<T>(IDialogViewModel viewModel) where T : Window, new()
        {
            T t = new()
            {
                DataContext = viewModel
            };

            return t.ShowDialog();
        }
    }
}
