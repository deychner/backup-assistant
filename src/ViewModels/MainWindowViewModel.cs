using BackupAssistant.DataModels;
using BackupAssistant.Models;
using BackupAssistant.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;
using System.IO.Abstractions;
using System.Threading;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly MainWindowModel _model;
        private readonly IFileSystem _fileSystem;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        public MainWindowViewModel(ISettingsService settingsService, IDialogService dialogService) : this(settingsService, dialogService, new FileSystem()) { }

        public MainWindowViewModel(ISettingsService settingsService, IDialogService dialogService, IFileSystem fileSystem)
        {
            _model = new MainWindowModel();

            _fileSystem = fileSystem;
            _settingsService = settingsService;
            _dialogService = dialogService;

            // Initialize filters if needed
            if (_settingsService.Filters == null)
            {
                _settingsService.Filters = new StringCollection();
                _settingsService.Save();
            }

            // Load filters from settings
            foreach (string? filter in _settingsService.Filters)
            {
                if (filter != null)
                {
                    this.FilterItems.Add(filter);
                }
            }

            // Initialize cancelable commands
            _runBackupCommand = new AsyncRelayCommand(async (CancellationToken token) => await RunBackup(token), CanRunBackup);

            // Load other settings
            this.Source = _settingsService.Source;
            this.Destination = _settingsService.Destination;
            this.BackupType = (BackupType)_settingsService.BackupType;
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

        public string Status
        {
            get { return _model.Status; }
            set
            {
                _model.Status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
    }
}
