using System.Diagnostics;
using System.Text;

namespace BackupAssistant.Services
{
    public class LogService : ILogService
    {
        private readonly StringBuilder _logMessage;
        private const string Source = "Backup Assistant";

        public LogService()
        {
            _logMessage = new StringBuilder();
        }

        public void AddToLogEntry(string message)
        {
            _logMessage?.AppendLine(message);
        }

        public void ClearLog()
        {
            _logMessage?.Clear();
        }

        public void WriteLogEntry()
        {
            if (_logMessage.Length > 0 && EventLog.SourceExists(Source))
            {
                using EventLog log = new();
                log.Source = Source;
                log.WriteEntry(_logMessage.ToString(), EventLogEntryType.Error);

                _logMessage.Clear();
            }
        }
    }
}
