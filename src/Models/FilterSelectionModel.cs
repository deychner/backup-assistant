using BackupAssistant.DataModels;
using System.Collections.ObjectModel;

namespace BackupAssistant.Models
{
    internal class FilterSelectionModel
    {
        public ObservableCollection<FilterItem> FilterItems { get; set; } = new ObservableCollection<FilterItem>();

        public FilterSelectionModel()
        {
            this.FilterItems.Add(new FilterItem() { IsChecked = true, Path = "testpath1" });
            this.FilterItems.Add(new FilterItem() { IsChecked = false, Path = "testpath2" });
            this.FilterItems.Add(new FilterItem() { IsChecked = true, Path = "testpath3" });
        }
    }
}
