using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Forms;

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
                // Do not reset filters when Source is being initialized or set to the same value
                if (!(string.IsNullOrEmpty(_model.Source) || _model.Source.Equals(value)))
                {
                    this.FilterItems = new ObservableCollection<string>();
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

        public void AddEditSource()
        {
            (DialogResult dialogResult, string selectedPath) = _dialogService.ShowFolderBrowserDialog(this.Source);

            if (dialogResult == DialogResult.OK)
            {
                this.Source = selectedPath;
            }
        }

        public void AddEditDestination()
        {
            (DialogResult dialogResult, string selectedPath) = _dialogService.ShowFolderBrowserDialog(this.Destination);

            if (dialogResult == DialogResult.OK)
            {
                this.Destination = selectedPath;
            }
        }
    }
}
