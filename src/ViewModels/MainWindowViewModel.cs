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
        private readonly ILogService _logService;

        public MainWindowViewModel(ISettingsService settingsService, IDialogService dialogService, ILogService logService) : this(settingsService, dialogService, logService, new FileSystem()) { }

        public MainWindowViewModel(ISettingsService settingsService, IDialogService dialogService, ILogService logService, IFileSystem fileSystem)
        {
            _model = new MainWindowModel();

            _fileSystem = fileSystem;
            _settingsService = settingsService;
            _dialogService = dialogService;
            _logService = logService;

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
            if (_fileSystem.Directory.Exists(_settingsService.Source))
            {
                this.Source = _settingsService.Source;
            }

            if (_fileSystem.Directory.Exists(_settingsService.Destination))
            {
                this.Destination = _settingsService.Destination;
            }

            this.BackupType = (BackupType)_settingsService.BackupType;
        }
    }
}
