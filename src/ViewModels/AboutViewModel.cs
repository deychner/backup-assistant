using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Reflection;

namespace BackupAssistant.ViewModels
{
    internal class AboutViewModel : ObservableObject
    {
        public static string ApplicationVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?.?.?.?";
            }
        }
    }
}
