using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _exitCommand;
        public IRelayCommand ExitCommand => _exitCommand ??= new RelayCommand(() => Environment.Exit(0));

        private RelayCommand? _aboutCommand;
        public IRelayCommand AboutCommand => _aboutCommand ??= new RelayCommand(() =>
        {
            About window = new();
            _ = window.ShowDialog();
        });
    }
}
