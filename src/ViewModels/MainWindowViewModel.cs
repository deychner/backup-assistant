using BackupAssistant.DataModels;
using BackupAssistant.Models;
using BackupAssistant.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupAssistant.ViewModels
{
    internal class MainWindowViewModel : ObservableObject
    {
        private readonly MainWindowModel _model;
        private readonly IDialogService _dialogService;

        public IRelayCommand AddEditSourceCommand => new RelayCommand(AddEditSource);
        public IRelayCommand AddEditDestinationCommand => new RelayCommand(AddEditDestination);
        public IRelayCommand EditFiltersCommand => new RelayCommand(() => EditFilters(new FilterSelectionViewModel()), CanEditFilters);
        public IAsyncRelayCommand RunBackupCommand => new AsyncRelayCommand(RunBackup, CanRunBackup);

        public string Source
        {
            get => _model.Source;
            set
            {
                if (!_model.Source.Equals(value))
                {
                    this.FilterItems = new ObservableCollection<string>();
                }

                _model.Source = value;
                OnPropertyChanged(nameof(Source));
                OnPropertyChanged(nameof(EditFiltersCommand));
                OnPropertyChanged(nameof(RunBackupCommand));
            }
        }

        public string Destination
        {
            get => _model.Destination;
            set
            {
                _model.Destination = value;
                OnPropertyChanged(nameof(Destination));
                OnPropertyChanged(nameof(RunBackupCommand));
            }
        }

        public BackupType BackupType
        {
            get => _model.BackupType;
            set
            {
                _model.BackupType = value;

                OnPropertyChanged(nameof(BackupType));
                OnPropertyChanged(nameof(RunBackupCommand));
            }
        }

        public ObservableCollection<string> FilterItems
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
                if (_model.Filters.Count > 0)
                {
                    return "/assets/filter_apply.png";
                }
                else
                {
                    return "/assets/filter.png";
                }
            }
        }

        public MainWindowViewModel(IDialogService dialogService)
        {
            _model = new MainWindowModel();
            _dialogService = dialogService;
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

        public void EditFilters(IDialogViewModel dialogViewModel)
        {
            dialogViewModel.Input = new FilterSelectionInput { RootPath = this.Source, ExistingFilters = this.FilterItems };

            bool? dialogResult = _dialogService.ShowDialog<FilterSelection>(dialogViewModel);

            if (dialogResult.HasValue && dialogResult.Value)
            {
                this.FilterItems = (ObservableCollection<string>)dialogViewModel.Output;
            }
        }

        public bool CanEditFilters()
        {
            return !string.IsNullOrEmpty(this.Source);
        }

        public Task RunBackup()
        {
            return Task.Delay(1000);
        }

        public bool CanRunBackup()
        {
            return !string.IsNullOrEmpty(this.Source) && !string.IsNullOrEmpty(this.Destination);
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
    }
}
