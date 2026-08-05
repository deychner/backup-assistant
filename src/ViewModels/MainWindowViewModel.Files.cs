using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _addEditSourceCommand;
        private RelayCommand? _addEditDestinationCommand;

        public IRelayCommand AddEditSourceCommand => _addEditSourceCommand ??= new RelayCommand(AddEditSource);
        public IRelayCommand AddEditDestinationCommand => _addEditDestinationCommand ??= new RelayCommand(AddEditDestination);

        public string Source
        {
            get => _model.Source;
            set
            {
                string oldValue = _model.Source;

                if (SetProperty(oldValue, value, _model, (m, v) => m.Source = v))
                {
                    // Do not reset filters when Source is being initialized (previously empty)
                    if (!string.IsNullOrEmpty(oldValue))
                    {
                        this.FilterItems = [];
                    }

                    // Update settings
                    _settingsService.Source = _model.Source;
                    _settingsService.Save();

                    // Update dependencies
                    this.EditFiltersCommand.NotifyCanExecuteChanged();
                    this.RunBackupCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public string Destination
        {
            get => _model.Destination;
            set
            {
                if (SetProperty(_model.Destination, value, _model, (m, v) => m.Destination = v))
                {
                    // Update settings
                    _settingsService.Destination = _model.Destination;
                    _settingsService.Save();

                    // Update dependencies
                    this.RunBackupCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public void AddEditSource()
        {
            string initialPath = GetOpenFolderDialogInitialPath(this.Source);
            (bool? dialogResult, string selectedPath) = _dialogService.ShowOpenFolderDialog(initialPath);

            if (dialogResult == true)
            {
                this.Source = selectedPath;
            }
        }

        public void AddEditDestination()
        {
            string initialPath = GetOpenFolderDialogInitialPath(this.Destination);
            (bool? dialogResult, string selectedPath) = _dialogService.ShowOpenFolderDialog(initialPath);

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
