using System.Collections.Generic;


namespace BackupAssistant.Core
{
    public interface IBackupStarter
    {
        string SourcePath { get; }

        string DestinationPath { get; }

        IList<string> Filters { get; }

        void PreProcess();

        void PostProcess();

        void ReportProgress(int progress);

        void ReportStatus(string status);

        void AddToLogEntry(string message);
    }
}
