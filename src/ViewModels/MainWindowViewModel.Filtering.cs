using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _editFiltersCommand;
        public IRelayCommand EditFiltersCommand => _editFiltersCommand ??= new RelayCommand(() => EditFilters(new FilterSelectionViewModel()), CanEditFilters);

        public ObservableCollection<string> FilterItems
        {
            get { return _model.Filters; }
            set
            {
                _model.Filters = value;
                OnPropertyChanged(nameof(FilterItems));

                // Update settings
                _settingsService.Filters.Clear();
                _settingsService.Filters.AddRange(_model.Filters.ToArray<string>());
                _settingsService.Save();

                // Update dependencies
                OnPropertyChanged(nameof(FilterImageSource));
            }
        }

        public string FilterImageSource
        {
            get
            {
                if (_model.Filters.Count > 0)
                {
                    return "/assets/filter_apply.png";
                }
                else
                {
                    return "/assets/filter.png";
                }
            }
        }

        public void EditFilters(IDialogViewModel dialogViewModel)
        {
            dialogViewModel.Input = new FilterSelectionInput { RootPath = this.Source, ExistingFilters = this.FilterItems };

            bool? dialogResult = _dialogService.ShowDialog<FilterSelection>(dialogViewModel);

            if (dialogResult.HasValue && dialogResult.Value)
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
