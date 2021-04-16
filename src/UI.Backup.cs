using BackupAssistant.Core;
using BackupAssistant.Properties;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        public string SourcePath
        {
            get { return Settings.Default.Source; }
        }

        public string DestinationPath
        {
            get { return Settings.Default.Destination; }
        }

        //private async void button1_Click(object sender, System.EventArgs e)
        //{
        //    button1.Enabled = false;
        //    await Task.Run(() => BackupAgent.RunFullBackup());
        //    button1.Enabled = true;
        //}
    }
}
