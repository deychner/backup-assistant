using BackupAssistant.DataModels;
using BackupAssistant.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System;
using System.Linq;

namespace BackupAssistant.ViewModels
{
    internal class FilterSelectionViewModel : ObservableRecipient
    {
        private FilterSelectionModel _model;
        private IFileSystem _fileSystem;

        public IRelayCommand SendFilterSelectionCommand { get; }

        public FilterSelectionViewModel() : this(new FileSystem())
        {

        }

        public FilterSelectionViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _model = new FilterSelectionModel
            {
                RootPath = @"D:\Products" // TODO: Get source path from MainWindowViewModel
            };
            PopulateFilterList();

            this.SendFilterSelectionCommand = new RelayCommand(SendFilterSelection);

            // TODO: I don't think I need this. This looks like it only replies to requests.
            this.IsActive = true;
        }

        public void PopulateFilterList()
        {
            if (!string.IsNullOrEmpty(_model.RootPath))
            {
                string[] directoriesToFilter = Array.Empty<string>();

                try
                {
                    directoriesToFilter = _fileSystem.Directory.GetDirectories(_model.RootPath);
                }
                catch
                {
                    // Do nothing
                }

                foreach (string d in directoriesToFilter)
                {
                    string shortName = d.Replace(_model.RootPath, "...");
                    _model.FilterItems.Add(new FilterItem { Path = shortName, IsChecked = false }); //TODO: Determine checked state
                }
            }
        }

        protected override void OnActivated()
        {
            // TODO: I don't think I need this. This looks like it only replies to requests.
            Messenger.Register<FilterSelectionViewModel, CurrentFiltersRequestMessage>(this, (r, m) => m.Reply(r.FilterItems));
        }

        protected override void OnDeactivated()
        {
            Messenger.Unregister<CurrentFiltersRequestMessage>(this);
        }

        public ObservableCollection<FilterItem> FilterItems
        {
            get { return _model.FilterItems; }
            set
            {
                _model.FilterItems = value;
                OnPropertyChanged(nameof(FilterItems));
            }
        }
        
        public void SendFilterSelection()
        {
            ObservableCollection<FilterItem> selectedItems = new(this.FilterItems.Where(x => x.IsChecked));
            Messenger.Send(new FiltersChangedMessage(selectedItems));
            // TODO: Deactivate and close
        }
    }
}
