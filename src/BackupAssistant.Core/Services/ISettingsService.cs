using System.Collections.Generic;

namespace BackupAssistant.Services
{
    public interface ISettingsService
    {
        int BackupType { get; set; }

        string Destination { get; set; }

        IList<string> Filters { get; set; }

        string Source { get; set; }

        void Save();
    }
}
