using Microsoft.UI.Xaml;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BackupAssistant.Services
{
    /// <summary>
    /// WinUI implementation of <see cref="IApplicationService"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApplicationService : IApplicationService
    {
        public string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?.?.?.?";

        public void Exit() => Application.Current.Exit();
    }
}
