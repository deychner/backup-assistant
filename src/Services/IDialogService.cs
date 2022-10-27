using System.Windows.Forms;

namespace BackupAssistant.Services
{
    public interface IDialogService
    {
        (DialogResult, string) ShowFolderBrowserDialog(string selectedPath = "");
    }
}
