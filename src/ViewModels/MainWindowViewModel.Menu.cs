using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _exitCommand;
        public IRelayCommand ExitCommand => _exitCommand ??= new RelayCommand(() => _applicationService.Shutdown());

        private RelayCommand? _aboutCommand;
        public IRelayCommand AboutCommand => _aboutCommand ??= new RelayCommand(() =>
            _dialogService.ShowDialog<About>(new AboutViewModel(_applicationService)));
    }
}
