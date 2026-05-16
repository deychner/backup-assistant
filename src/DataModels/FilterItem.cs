using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupAssistant.DataModels
{
    public partial class FilterItem : ObservableObject
    {
        [ObservableProperty]
        public bool isChecked = false;
        
        [ObservableProperty]
        public string path = string.Empty;
    }
}

