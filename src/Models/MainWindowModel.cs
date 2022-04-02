using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    internal class MainWindowModel
    {
        public ObservableCollection<string> Filters { get; set; }
        
        public MainWindowModel()
        {
            this.Filters = new ObservableCollection<string>();
        }
    }
}
