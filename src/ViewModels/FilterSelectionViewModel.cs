using BackupAssistant.DataModels;
using BackupAssistant.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;

namespace BackupAssistant.ViewModels
{
    internal class FilterSelectionViewModel : ObservableObject, IDialogViewModel
    {
        private readonly FilterSelectionModel _model;
        private readonly IFileSystem _fileSystem;

        public FilterSelectionViewModel() : this(new FileSystem())
        {

        }

        public FilterSelectionViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _model = new FilterSelectionModel();
        }

        public void PopulateFilterList()
        {
            _model.FilterItems.Clear();
            
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

        public ObservableCollection<FilterItem> FilterItems
        {
            get { return _model.FilterItems; }
            set
            {
                _model.FilterItems = value;
                OnPropertyChanged(nameof(FilterItems));
            }
        }

        public object Input
        {
            set
            {
                if (value != null && !_model.RootPath.Equals(value.ToString()))
                {
                    _model.RootPath = value.ToString()!;
                    PopulateFilterList();
                }
            }
        }

        public object Output => _model.FilterItems;
    }
}
