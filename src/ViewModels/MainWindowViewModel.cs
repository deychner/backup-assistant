using BackupAssistant.DataModels;
using BackupAssistant.Models;
using BackupAssistant.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace BackupAssistant.ViewModels
{
    internal class MainWindowViewModel : ObservableRecipient
    {
        private readonly MainWindowModel _model;
        private readonly IDialogService _dialogService;

        public IRelayCommand AddEditSourceCommand => new RelayCommand(AddEditSource);
        public IRelayCommand AddEditDestinationCommand => new RelayCommand(AddEditDestination);

        public MainWindowViewModel(IDialogService dialogService)
        {
            _model = new MainWindowModel();
            _dialogService = dialogService;

            this.IsActive = true;
        }

        protected override void OnActivated()
        {
            Messenger.Register<MainWindowViewModel, FiltersChangedMessage>(this, (r, m) => r.FilterItems = m.Value);
        }

        public string Source
        {
            get => _model.Source;
            set
            {
                _model.Source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public string Destination
        {
            get => _model.Destination;
            set
            {
                _model.Destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }

        public void AddEditSource()
        {
            (DialogResult dialogResult, string selectedPath) = _dialogService.ShowFolderBrowserDialog(this.Source);

            if (dialogResult == DialogResult.OK)
            {
                this.Source = selectedPath;
            }
        }

        public void AddEditDestination()
        {
            (DialogResult dialogResult, string selectedPath) = _dialogService.ShowFolderBrowserDialog(this.Destination);

            if (dialogResult == DialogResult.OK)
            {
                this.Destination = selectedPath;
            }
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
