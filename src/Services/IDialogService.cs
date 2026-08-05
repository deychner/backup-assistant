using System.Windows;

namespace BackupAssistant.Services
{
    public interface IDialogService
    {
        (bool?, string) ShowOpenFolderDialog(string selectedPath = "");

        bool? ShowDialog<T>(object viewModel) where T : Window, new();
    }
}
