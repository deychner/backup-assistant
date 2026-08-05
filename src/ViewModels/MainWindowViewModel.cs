using BackupAssistant.DataModels;
using BackupAssistant.Models;
using BackupAssistant.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly MainWindowModel _model;
        private readonly IFileSystem _fileSystem;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;
        private readonly IApplicationService _applicationService;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IBackupService _backupService;

        public MainWindowModel Model => _model;

        public MainWindowViewModel(
            IBackupService backupService,
            ISettingsService settingsService,
            IDialogService dialogService,
            IApplicationService applicationService,
            ILogger<MainWindowViewModel> logger) : this(
                backupService,
                settingsService,
                dialogService,
                applicationService,
                logger,
                new FileSystem())
        { }

        public MainWindowViewModel(
            IBackupService backupService,
            ISettingsService settingsService,
            IDialogService dialogService,
            IApplicationService applicationService,
            ILogger<MainWindowViewModel> logger,
            IFileSystem fileSystem)
        {
            _model = new MainWindowModel();

            _backupService = backupService;
            _settingsService = settingsService;
            _dialogService = dialogService;
            _applicationService = applicationService;
            _logger = logger;
            _fileSystem = fileSystem;

            // Initialize filters if needed
            if (_settingsService.Filters == null)
            {
                _settingsService.Filters = [];
                _settingsService.Save();
            }

            // Load filters from settings directly into the backing model. Avoid the FilterItems
            // setter here, since it re-saves settings and would rewrite user.config on every launch.
            foreach (string? filter in _settingsService.Filters)
            {
                if (filter != null)
                {
                    _model.Filters.Add(filter);
                }
            }

            // Load source, destination, and backup type directly into the backing model for the
            // same reason. Their existence will be checked when the user tries to backup.
            _model.Source = _settingsService.Source;
            _model.Destination = _settingsService.Destination;
            _model.BackupType = (BackupType)_settingsService.BackupType;
        }
    }
}
