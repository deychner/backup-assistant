using BackupAssistant.DataModels;
using BackupAssistant.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private AsyncRelayCommand? _runBackupCommand;
        public IAsyncRelayCommand RunBackupCommand => _runBackupCommand ??= new AsyncRelayCommand(async token => await RunBackupAsync(token), CanRunBackup);

        private ICommand? _cancelRunBackupCommand;
        public ICommand CancelRunBackupCommand => _cancelRunBackupCommand ??= this.RunBackupCommand.CreateCancelCommand();

        public async Task RunBackupAsync(CancellationToken token)
        {
            if (!_fileSystem.Directory.Exists(this.Source))
            {
                _logger.LogError("Backup failed. The source directory '{this.Source}' does not exist.", this.Source);

                this.Status = "The source directory does not exist.";
                return;
            }

            if (!_fileSystem.Directory.Exists(this.Destination))
            {
                _logger.LogError("Backup failed. The destination directory '{this.Destination}' does not exist.", this.Destination);

                this.Status = "The destination directory does not exist.";
                return;
            }

            IProgress<BackupProgress> progress = new Progress<BackupProgress>(p =>
            {
                if (p.Progress.HasValue)
                    this.Progress = p.Progress.Value;

                if (p.IsIndeterminate.HasValue)
                    this.ProgressBarIsIndeterminate = p.IsIndeterminate.Value;

                if (!string.IsNullOrEmpty(p.Status))
                    this.Status = p.Status;
            });

            try
            {
                switch (this.BackupType)
                {
                    case BackupType.Full:
                        await Task.Run(async () => await _backupService.RunFullBackupAsync(this.Source, this.Destination, this.FilterItems, progress, token), token);
                        break;
                    case BackupType.Incremental:
                        await Task.Run(async () => await _backupService.RunIncrementalBackupAsync(this.Source, this.Destination, this.FilterItems, progress, token), token);
                        break;
                    default:
                        // do nothing
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                this.Status = "Backup was canceled.";
            }
        }

        public bool CanRunBackup()
        {
            return !string.IsNullOrEmpty(this.Source) && !string.IsNullOrEmpty(this.Destination) && !_runBackupCommand!.IsRunning;
        }

        public BackupType BackupType
        {
            get => _model.BackupType;
            set
            {
                if (SetProperty(_model.BackupType, value, _model, (m, v) => m.BackupType = v))
                {
                    // Update settings
                    _settingsService.BackupType = (int)_model.BackupType;
                    _settingsService.Save();
                }
            }
        }

        public int Progress
        {
            get { return _model.Progress; }
            set
            {
                int clamped = Math.Clamp(value, 0, 100);
                _ = SetProperty(_model.Progress, clamped, _model, (m, v) => m.Progress = v);
            }
        }

        public bool ProgressBarIsIndeterminate
        {
            get { return _model.ProgressBarIsIndeterminate; }
            set => SetProperty(_model.ProgressBarIsIndeterminate, value, _model, (m, v) => m.ProgressBarIsIndeterminate = v);
        }

        public string Status
        {
            get { return _model.Status; }
            set => SetProperty(_model.Status, value, _model, (m, v) => m.Status = v);
        }
    }
}
