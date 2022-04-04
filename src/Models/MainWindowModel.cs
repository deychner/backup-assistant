using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    internal class MainWindowModel
    {
        public ObservableCollection<string> Filters { get; private set; } = new ObservableCollection<string>();

        public int Progress { get; set; } = 0;

        public MainWindowModel()
        {

        }
    }
}
