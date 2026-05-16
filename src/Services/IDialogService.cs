using BackupAssistant.ViewModels;
using System.Threading.Tasks;

namespace BackupAssistant.Services
{
    public interface IDialogService
    {
        Task<(bool?, string)> ShowOpenFolderDialog(string selectedPath = "");

        bool? ShowDialog<T>(IDialogViewModel viewModel) where T : new();
    }
}


