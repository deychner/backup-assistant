using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _exitCommand;
        public IRelayCommand ExitCommand => _exitCommand ??= new RelayCommand(_applicationService.Shutdown);

        private AsyncRelayCommand? _aboutCommand;
        public IAsyncRelayCommand AboutCommand => _aboutCommand ??= new AsyncRelayCommand(_dialogService.ShowAboutDialogAsync);
    }
}
