using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public IAsyncRelayCommand RunBackupCommand => new AsyncRelayCommand(RunBackup, CanRunBackup);

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

        public Task RunBackup()
        {
            return Task.Delay(1000);
        }

        public bool CanRunBackup()
        {
            return !string.IsNullOrEmpty(this.Source) && !string.IsNullOrEmpty(this.Destination);
        }
    }
}
