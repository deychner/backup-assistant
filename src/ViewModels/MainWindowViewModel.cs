using BackupAssistant.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace BackupAssistant.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        private MainWindowModel _model;

        public MainWindowViewModel()
        {
            _model = new MainWindowModel();
        }

        public string FilterImageSource
        {
            get
            {
                if (_model.Filters.Count == 0)
                {
                    return "/assets/filter.png";
                }
                else
                {
                    return "/assets/filter_apply.png";
                }
            }
        }
    }
}
