using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;

namespace BackupAssistant.Services
{
    [ExcludeFromCodeCoverage]
    public class ApplicationService : IApplicationService
    {
        public string ApplicationVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?.?.?.?";
            }
        }

        public void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}
