namespace BackupAssistant.Services
{
    public interface ILogService
    {
        void AddToLogEntry(string message);

        void WriteLogEntry();
    }
}
