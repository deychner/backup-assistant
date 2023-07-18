using System.Collections.Specialized;

namespace BackupAssistant.Services
{
    public interface ISettingsService
    {
        int BackupType { get; set; }

        string Destination { get; set; }

        StringCollection Filters { get; set; }

        string Source { get; set; }

        // Intentionally disabled, since we do not want users changing this value
        //bool UpgradeRequired { get; set; }

        void Save();
    }
}
