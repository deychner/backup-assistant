using BackupAssistant.DataModels;
using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    public class FilterSelectionModel
    {
        public string RootPath { get; set; } = string.Empty;
        public ObservableCollection<FilterItem> FilterItems { get; set; } = new ObservableCollection<FilterItem>();

        public FilterSelectionModel()
        {

        }
    }
}
