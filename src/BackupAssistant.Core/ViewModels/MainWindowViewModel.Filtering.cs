using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private AsyncRelayCommand? _editFiltersCommand;

        public IAsyncRelayCommand EditFiltersCommand => _editFiltersCommand ??= new AsyncRelayCommand(
            () => EditFiltersAsync(new FilterSelectionViewModel(_fileSystem)),
            CanEditFilters);

        public ObservableCollection<string> FilterItems
        {
            get => _model.Filters;
            set
            {
                _model.Filters = value;
                OnPropertyChanged(nameof(FilterItems));

                // Update settings
                _settingsService.Filters = [.. _model.Filters];
                _settingsService.Save();

                // Update dependencies
                OnPropertyChanged(nameof(HasFilters));
                OnPropertyChanged(nameof(FilterSummary));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the backup is narrowed to a subset of folders.
        /// </summary>
        public bool HasFilters => _model.Filters.Count > 0;

        /// <summary>
        /// Gets a plain-language description of the current folder selection, shown next to the
        /// button that opens the folder selection dialog. An empty selection means everything
        /// under the source folder is backed up.
        /// </summary>
        public string FilterSummary => _model.Filters.Count switch
        {
            0 => "All folders",
            1 => "1 folder selected",
            int count => $"{count} folders selected"
        };

        public async Task EditFiltersAsync(FilterSelectionViewModel dialogViewModel)
        {
            dialogViewModel.Input = new FilterSelectionInput { RootPath = this.Source, ExistingFilters = this.FilterItems };

            if (await _dialogService.ShowFilterSelectionDialogAsync(dialogViewModel))
            {
                this.FilterItems = (ObservableCollection<string>)dialogViewModel.Output;
            }
        }

        public bool CanEditFilters()
        {
            return !string.IsNullOrEmpty(this.Source);
        }
    }
}
