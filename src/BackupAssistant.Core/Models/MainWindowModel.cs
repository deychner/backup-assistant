using BackupAssistant.DataModels;
using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    public class MainWindowModel
    {
        public string Source { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public BackupType BackupType { get; set; }

        public ObservableCollection<string> Filters { get; set; } = [];

        public int Progress { get; set; }

        public bool ProgressBarIsIndeterminate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
