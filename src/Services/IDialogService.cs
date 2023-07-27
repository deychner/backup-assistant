using BackupAssistant.ViewModels;
using System.Windows;
using System.Windows.Forms;

namespace BackupAssistant.Services
{
    public interface IDialogService
    {
        (DialogResult, string) ShowFolderBrowserDialog(string selectedPath = "");

        bool? ShowDialog<T>(IDialogViewModel viewModel) where T : Window, new();
    }
}
