using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace BackupAssistant.DataModels
{
    internal sealed class FiltersChangedMessage : ValueChangedMessage<ObservableCollection<FilterItem>>
    {
        public FiltersChangedMessage(ObservableCollection<FilterItem> value) : base(value)
        {

        }
    }
}
