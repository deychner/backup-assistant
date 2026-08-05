using BackupAssistant.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupAssistant.ViewModels
{
    public class AboutViewModel(IApplicationService applicationService) : ObservableObject
    {
        public string ApplicationVersion => applicationService.ApplicationVersion;
    }
}
