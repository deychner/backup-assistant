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
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IBackupService _backupService;

        public MainWindowModel Model => _model;

        public MainWindowViewModel(
            IBackupService backupService,
            ISettingsService settingsService,
            IDialogService dialogService,
            ILogger<MainWindowViewModel> logger) : this(
                backupService,
                settingsService,
                dialogService,
                logger,
                new FileSystem()) { }

        public MainWindowViewModel(
            IBackupService backupService,
            ISettingsService settingsService,
            IDialogService dialogService,
            ILogger<MainWindowViewModel> logger,
            IFileSystem fileSystem)
        {
            _model = new MainWindowModel();

            _backupService = backupService;
            _settingsService = settingsService;
            _dialogService = dialogService;
            _logger = logger;
            _fileSystem = fileSystem;

            // Initialize filters if needed
            if (_settingsService.Filters == null)
            {
                _settingsService.Filters = [];
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

            // Load source and destination from settings, regardless of whether they exist.
            // Their existence will be checked when the user tries to backup.
            this.Source = _settingsService.Source;
            this.Destination = _settingsService.Destination;

            this.BackupType = (BackupType)_settingsService.BackupType;
        }
    }
}
