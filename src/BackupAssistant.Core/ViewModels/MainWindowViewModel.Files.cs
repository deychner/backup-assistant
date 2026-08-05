using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private AsyncRelayCommand? _addEditSourceCommand;
        private AsyncRelayCommand? _addEditDestinationCommand;

        public IAsyncRelayCommand AddEditSourceCommand => _addEditSourceCommand ??= new AsyncRelayCommand(AddEditSourceAsync);
        public IAsyncRelayCommand AddEditDestinationCommand => _addEditDestinationCommand ??= new AsyncRelayCommand(AddEditDestinationAsync);

        public string Source
        {
            get => _model.Source;
            set
            {
                if (string.Equals(_model.Source, value, StringComparison.Ordinal))
                {
                    return;
                }

                // Filters name folders beneath the previous source, so they cannot carry over.
                // Nothing to clear when the source is only now being set for the first time.
                if (!string.IsNullOrEmpty(_model.Source))
                {
                    this.FilterItems = [];
                }

                _model.Source = value;
                OnPropertyChanged(nameof(Source));

                // Update settings
                _settingsService.Source = value;
                _settingsService.Save();

                // Update dependencies
                this.EditFiltersCommand.NotifyCanExecuteChanged();
                this.RunBackupCommand.NotifyCanExecuteChanged();
            }
        }

        public string Destination
        {
            get => _model.Destination;
            set
            {
                if (SetProperty(_model.Destination, value, _model, (model, v) => model.Destination = v))
                {
                    // Update settings
                    _settingsService.Destination = value;
                    _settingsService.Save();

                    // Update dependencies
                    this.RunBackupCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public async Task AddEditSourceAsync()
        {
            string? selectedPath = await _dialogService.ShowFolderPickerAsync();

            if (!string.IsNullOrEmpty(selectedPath))
            {
                this.Source = selectedPath;
            }
        }

        public async Task AddEditDestinationAsync()
        {
            string? selectedPath = await _dialogService.ShowFolderPickerAsync();

            if (!string.IsNullOrEmpty(selectedPath))
            {
                this.Destination = selectedPath;
            }
        }
    }
}
