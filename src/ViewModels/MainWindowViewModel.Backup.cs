using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private AsyncRelayCommand _runBackupCommand;
        public IAsyncRelayCommand RunBackupCommand => _runBackupCommand ??= new AsyncRelayCommand(async (CancellationToken token) => await RunBackup(token), CanRunBackup);

        public ICommand CancelRunBackupCommand => this.RunBackupCommand.CreateCancelCommand();

        public async Task RunBackup(CancellationToken token)
        {
            try
            {
                await Task.Run(() => RunBackupInternal(token), token);
            }
            catch (OperationCanceledException)
            {
                // No action is needed
            }
        }

        public bool CanRunBackup()
        {
            return !string.IsNullOrEmpty(this.Source) && !string.IsNullOrEmpty(this.Destination) && !_runBackupCommand.IsRunning;
        }

        internal void RunBackupInternal(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
            }
        }

        public BackupType BackupType
        {
            get => _model.BackupType;
            set
            {
                _model.BackupType = value;
                OnPropertyChanged(nameof(BackupType));

                // Update settings
                _settingsService.BackupType = (int)_model.BackupType;
                _settingsService.Save();

                // Update dependencies
                OnPropertyChanged(nameof(RunBackupCommand));
            }
        }
    }
}
