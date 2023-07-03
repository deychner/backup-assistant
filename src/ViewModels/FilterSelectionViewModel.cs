using BackupAssistant.DataModels;
using BackupAssistant.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;

namespace BackupAssistant.ViewModels
{
    internal class FilterSelectionViewModel : ObservableObject, IDialogViewModel
    {
        private readonly FilterSelectionModel _model;
        private readonly IFileSystem _fileSystem;

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
                if (value != null && value is FilterSelectionInput f)
                {
                    _model.RootPath = f.RootPath;

                    PopulateFilterList(f.ExistingFilters ?? new List<string>());
                }
            }
        }

        public object Output => new ObservableCollection<string>(from f in _model.FilterItems where f.IsChecked select f.Path);

        public FilterSelectionViewModel() : this(new FileSystem()) { }

        public FilterSelectionViewModel(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            _model = new FilterSelectionModel();
        }

        public void PopulateFilterList(IEnumerable<string> existingFilters)
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

                foreach (string d in directoriesToFilter.Where(d => !_fileSystem.DirectoryInfo.New(d).Attributes.HasFlag(System.IO.FileAttributes.Hidden)))
                {
                    string shortName = d.Replace(_model.RootPath, "...");

                    _model.FilterItems.Add(new FilterItem
                    {
                        Path = shortName,
                        IsChecked = existingFilters.Where(f => f.Equals(shortName)).Any()
                    });
                }
            }
        }
    }
}
