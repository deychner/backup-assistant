using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using System;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _exitCommand;
        public IRelayCommand ExitCommand => _exitCommand ??= new RelayCommand(() => Application.Current.Exit());

        private RelayCommand? _aboutCommand;
        public IRelayCommand AboutCommand => _aboutCommand ??= new RelayCommand(() =>
        {
            About window = new();
            window.Activate();
        });
    }
}

