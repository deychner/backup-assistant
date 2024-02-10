using BackupAssistant.ViewModels;
using System.Windows;

namespace BackupAssistant.Services
{
    public interface IDialogService
    {
        (bool?, string) ShowOpenFolderDialog(string selectedPath = "");

        bool? ShowDialog<T>(IDialogViewModel viewModel) where T : Window, new();
    }
}
