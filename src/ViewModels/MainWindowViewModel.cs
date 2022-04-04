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

        public int Progress
        {
            get { return _model.Progress; }
            set
            {
                if (value < 0)
                {
                    _model.Progress = 0;
                }
                else if (value > 100)
                {
                    _model.Progress = 100;
                }
                else
                {
                    _model.Progress = value;
                }
                
                OnPropertyChanged(nameof(Progress));
            }
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
