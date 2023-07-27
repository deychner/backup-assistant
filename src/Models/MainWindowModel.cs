using BackupAssistant.DataModels;
using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    public class MainWindowModel
    {
        public string Source = string.Empty;
        public string Destination = string.Empty;
        public BackupType BackupType;
        public ObservableCollection<string> Filters { get; set; } = new ObservableCollection<string>();

        public int Progress { get; set; } = 0;
        public bool ProgressBarIsIndeterminate { get; set; } = false;
        public string Status { get; set; } = string.Empty;

        public MainWindowModel()
        {

        }
    }
}
