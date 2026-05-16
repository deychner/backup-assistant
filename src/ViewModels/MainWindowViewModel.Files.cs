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

        public IAsyncRelayCommand AddEditSourceCommand => _addEditSourceCommand ??= new AsyncRelayCommand(AddEditSource);
        public IAsyncRelayCommand AddEditDestinationCommand => _addEditDestinationCommand ??= new AsyncRelayCommand(AddEditDestination);

        public string Source
        {
            get => _model.Source;
            set
            {
                // Do not reset filters when Source is being initialized or set to the same value
                if (!(string.IsNullOrEmpty(_model.Source) || _model.Source.Equals(value)))
                {
                    this.FilterItems = [];
                }

                _model.Source = value;
                OnPropertyChanged(nameof(Source));

                // Update settings
                _settingsService.Source = _model.Source;
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
                _model.Destination = value;
                OnPropertyChanged(nameof(Destination));

                // Update settings
                _settingsService.Destination = _model.Destination;
                _settingsService.Save();

                // Update dependencies
                this.RunBackupCommand.NotifyCanExecuteChanged();
            }
        }

        public async Task AddEditSource()
        {
            string initialPath = GetOpenFolderDialogInitialPath(this.Source);
            (bool? dialogResult, string selectedPath) = await _dialogService.ShowOpenFolderDialog(initialPath);

            if (dialogResult == true)
            {
                this.Source = selectedPath;
            }
        }

        public async Task AddEditDestination()
        {
            string initialPath = GetOpenFolderDialogInitialPath(this.Destination);
            (bool? dialogResult, string selectedPath) = await _dialogService.ShowOpenFolderDialog(initialPath);

            if (dialogResult == true)
            {
                this.Destination = selectedPath;
            }
        }

        internal string GetOpenFolderDialogInitialPath(string suggestedPath)
        {
            return !_fileSystem.Directory.Exists(suggestedPath)
                ? _fileSystem.Path.GetPathRoot(Environment.SystemDirectory) ?? @"C:\"
                : suggestedPath;
        }
    }
}

