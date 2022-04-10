using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;

namespace BackupAssistant.DataModels
{
    internal sealed class CurrentFiltersRequestMessage : RequestMessage<ObservableCollection<FilterItem>>
    {
    }
}
