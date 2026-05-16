using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private RelayCommand? _editFiltersCommand;
        private RelayCommand? _closeFiltersPaneCommand;
        private ObservableCollection<FilterItem> _filterItemsForPane = [];

        public IRelayCommand EditFiltersCommand => _editFiltersCommand ??= new RelayCommand(EditFilters, CanEditFilters);
        public IRelayCommand CloseFiltersPaneCommand => _closeFiltersPaneCommand ??= new RelayCommand(CloseFiltersPane);

        public bool IsFiltersPaneOpen
        {
            get { return _model.IsFiltersPaneOpen; }
            set
            {
                _model.IsFiltersPaneOpen = value;
                OnPropertyChanged(nameof(IsFiltersPaneOpen));
            }
        }

        public string FilterIconGlyph => "\uEF4B";  // Filter icon from Segoe MDL2 Assets

        public string FilterCountText
        {
            get
            {
                return _model.Filters.Count > 0 ? $"Filters ({_model.Filters.Count})" : "Edit Filters";
            }
        }

        public ObservableCollection<FilterItem> FilterItemsForPane
        {
            get { return _filterItemsForPane; }
            set
            {
                _filterItemsForPane = value;
                OnPropertyChanged(nameof(FilterItemsForPane));
            }
        }

        public ObservableCollection<string> FilterItems
        {
            get { return _model.Filters; }
            set
            {
                _model.Filters = value;
                OnPropertyChanged(nameof(FilterItems));

                // Update settings
                _settingsService.Filters.Clear();
                _settingsService.Filters.AddRange([.. _model.Filters]);
                _settingsService.Save();

                // Update UI dependencies
                OnPropertyChanged(nameof(FilterCountText));
            }
        }

        public void EditFilters()
        {
            PopulateFilterPane();
            this.IsFiltersPaneOpen = true;
        }

        private void PopulateFilterPane()
        {
            if (string.IsNullOrEmpty(this.Source))
                return;

            var paneItems = new ObservableCollection<FilterItem>();

            try
            {
                var directories = _fileSystem.Directory.GetDirectories(this.Source);

                foreach (var directory in directories)
                {
                    var directoryInfo = _fileSystem.DirectoryInfo.New(directory);
                    
                    // Skip hidden directories
                    if (directoryInfo.Attributes.HasFlag(System.IO.FileAttributes.Hidden))
                        continue;

                    string shortName = directory.Replace(this.Source, "...");
                    
                    // Check if this directory is in the current filters
                    bool isChecked = this.FilterItems.Contains(shortName);

                    paneItems.Add(new FilterItem
                    {
                        Path = shortName,
                        IsChecked = isChecked
                    });
                }
            }
            catch
            {
                // Silently handle errors (e.g., inaccessible directory)
            }

            this.FilterItemsForPane = paneItems;
        }

        public void CloseFiltersPane()
        {
            // When closing, sync the checked items from the pane back to FilterItems
            var selectedFilters = _filterItemsForPane
                .Where(f => f.IsChecked)
                .Select(f => f.Path)
                .ToList();

            var newFilterCollection = new ObservableCollection<string>(selectedFilters);
            this.FilterItems = newFilterCollection;
            
            this.IsFiltersPaneOpen = false;
        }

        public bool CanEditFilters()
        {
            return !string.IsNullOrEmpty(this.Source);
        }
    }
}



