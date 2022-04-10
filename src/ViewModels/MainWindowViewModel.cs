using BackupAssistant.DataModels;
using BackupAssistant.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace BackupAssistant.ViewModels
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        private MainWindowModel _model;

        public MainWindowViewModel()
        {
            _model = new MainWindowModel();

            this.IsActive = true;
        }

        protected override void OnActivated()
        {
            Messenger.Register<MainWindowViewModel, FiltersChangedMessage>(this, (r, m) => r.FilterItems = m.Value);
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

        public ObservableCollection<FilterItem> FilterItems
        {
            get { return _model.Filters; }
            set
            {
                _model.Filters = value;
                OnPropertyChanged(nameof(FilterItems));
                OnPropertyChanged(nameof(FilterImageSource));
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
