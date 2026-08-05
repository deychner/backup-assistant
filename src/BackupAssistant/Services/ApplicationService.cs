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
        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?.?.?.?";

        /// <summary>
        /// WinUI's equivalent of WPF's <c>Application.Current.Shutdown()</c>. This runs the normal
        /// shutdown path, so Serilog's <c>AddSerilog(dispose: true)</c> registration still flushes.
        /// </summary>
        public void Shutdown() => Application.Current.Exit();
    }
}
