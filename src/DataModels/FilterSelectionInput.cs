using System.Collections.ObjectModel;

namespace BackupAssistant.DataModels
{
    public class FilterSelectionInput
    {
        public string RootPath { get; set; } = string.Empty;

        public ObservableCollection<FilterItem> ExistingFilters { get; set; } = new ObservableCollection<FilterItem>();
    }
}
