using BackupAssistant.DataModels;
using BackupAssistant.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace BackupAssistant.ViewModels
{
    internal class FilterSelectionViewModel : ObservableRecipient
    {
        private FilterSelectionModel _model;

        public IRelayCommand SendFilterSelectionCommand { get; }

        public FilterSelectionViewModel()
        {
            _model = new FilterSelectionModel();

            this.SendFilterSelectionCommand = new RelayCommand(SendFilterSelection);

            this.IsActive = true;
        }

        protected override void OnActivated()
        {
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
            Messenger.Send(new FiltersChangedMessage(this.FilterItems));
        }
    }
}
