using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public IRelayCommand AddEditSourceCommand => new RelayCommand(AddEditSource);
        public IRelayCommand AddEditDestinationCommand => new RelayCommand(AddEditDestination);

        public string Source
        {
            get => _model.Source;
            set
            {
                _model.Source = value;
                OnPropertyChanged(nameof(Source));

                // Update settings
                _settingsService.Source = _model.Source;
                _settingsService.Save();

                // Update dependencies
                if (!_model.Source.Equals(value))
                {
                    this.FilterItems = new ObservableCollection<string>();
                }

                OnPropertyChanged(nameof(EditFiltersCommand));
                OnPropertyChanged(nameof(RunBackupCommand));
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
                OnPropertyChanged(nameof(RunBackupCommand));
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
