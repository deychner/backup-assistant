using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    internal class MainWindowModel
    {
        public string Source = string.Empty;
        public string Destination = string.Empty;
        public ObservableCollection<string> Filters { get; set; } = new ObservableCollection<string>();

        public int Progress { get; set; } = 0;

        public MainWindowModel()
        {

        }
    }
}
