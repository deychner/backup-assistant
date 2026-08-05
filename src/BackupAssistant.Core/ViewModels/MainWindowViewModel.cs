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

            // Load filters from settings, ignoring any blank entry left behind by an older settings file
            foreach (string filter in _settingsService.Filters)
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    _model.Filters.Add(filter);
                }
            }

            // Load source and destination from settings, regardless of whether they exist.
            // Their existence will be checked when the user tries to backup.
            // These assign the model directly rather than going through the properties, so that
            // merely starting up does not write the settings straight back out to disk again.
            _model.Source = _settingsService.Source;
            _model.Destination = _settingsService.Destination;
            _model.BackupType = (BackupType)_settingsService.BackupType;
        }
    }
}
