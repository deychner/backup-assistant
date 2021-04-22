using BackupAssistant.Core;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        private StringBuilder _logMessage = null;

        public void AddToLogEntry(string message)
        {
            if (_logMessage != null)
            {
                _logMessage.AppendLine(message);
            }
        }

        private void WriteLogEntry()
        {
            if (_logMessage != null && _logMessage.Length == 0)
            {
                return;
            }

            using EventLog log = new EventLog();
            log.Source = "Backup Assistant";
            log.WriteEntry(_logMessage.ToString(), EventLogEntryType.Error);
        }
    }
}
