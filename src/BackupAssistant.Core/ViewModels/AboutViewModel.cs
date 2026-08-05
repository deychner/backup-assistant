using BackupAssistant.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BackupAssistant.ViewModels
{
    public class AboutViewModel(IApplicationService applicationService) : ObservableObject
    {
        /// <summary>
        /// Gets the version of the running application, for display in the about dialog.
        /// </summary>
        public string ApplicationVersion => applicationService.ApplicationVersion;
    }
}
