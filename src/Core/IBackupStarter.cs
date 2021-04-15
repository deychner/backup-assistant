using System.Collections.ObjectModel;

namespace BackupAssistant.Core
{
    public interface IBackupStarter
    {
        string SourcePath { get; }

        string DestinationPath { get; }

        ReadOnlyCollection<string> Filters { get; }

        void PreProcess();

        void PostPorcess();

        void ReportProgress(int progress);

        void ReportStatus(string status);

        void AddToLogEntry(string message);
    }
}
