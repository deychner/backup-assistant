using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {

        internal void RunIncrementalBackupInternal(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
