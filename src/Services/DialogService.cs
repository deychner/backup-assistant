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
    }
}
