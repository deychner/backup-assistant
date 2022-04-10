using BackupAssistant.DataModels;
using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    internal class MainWindowModel
    {
        public ObservableCollection<FilterItem> Filters { get; set; } = new ObservableCollection<FilterItem>();

        public int Progress { get; set; } = 0;

        public MainWindowModel()
        {

        }
    }
}
