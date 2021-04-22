using BackupAssistant.Core;
using BackupAssistant.Properties;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        private enum BackupType : int
        {
            Incremental = 0,
            Full
        }

        private BackupAgent _backupAgent = null;

        public string SourcePath
        {
            get { return Settings.Default.Source; }
        }

        public string DestinationPath
        {
            get { return Settings.Default.Destination; }
        }

        private void UI_ComboBox_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.BackupType = this.UI_ComboBox_Type.SelectedIndex;
        }

        private async void UI_Button_Backup_Click(object sender, EventArgs e)
        {
            _logMessage.Clear();

            this.UI_Button_Backup.Enabled = false;
            this.UI_Button_Cancel.Enabled = true;

            try
            {
                switch ((BackupType)Settings.Default.BackupType)
                {
                    case BackupType.Incremental:
                        this.UI_Button_Backup.Enabled = false;
                        await Task.Run(() => _backupAgent.RunIncrementalBackup());
                        break;
                    case BackupType.Full:
                        this.UI_Button_Backup.Enabled = false;
                        await Task.Run(() => _backupAgent.RunFullBackup());
                        break;
                    default:
                        // Do nothing
                        break;
                }
            }
            catch (InvalidOperationException o)
            {
                HandleException(o);
            }
            catch (ArgumentException a)
            {
                HandleException(a);
            }
            finally
            {
                WriteLogEntry();
            }

            this.UI_Button_Backup.Enabled = true;
            this.UI_Button_Cancel.Enabled = false;
        }

        private void UI_Button_Cancel_Click(object sender, EventArgs e)
        {
            _backupAgent.Cancel();
        }
    }
}
