using BackupAssistant.ViewModels;
using System.Windows;
using System.Windows.Forms;

namespace BackupAssistant.Services
{
    public class DialogService : IDialogService
    {
        public (DialogResult, string) ShowFolderBrowserDialog(string selectedPath = "")
        {
            using FolderBrowserDialog dialog = new()
            {
                SelectedPath = selectedPath
            };

            return (dialog.ShowDialog(), dialog.SelectedPath);
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
