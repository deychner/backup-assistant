using BackupAssistant.Core;
using BackupAssistant.Properties;
using System;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        private void UI_MenuStrip_Exit_Click(object sender, EventArgs e)
        {
            Exit(0);
        }

        private void UI_MenuStrip_About_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Exit(object sender, EventArgs e)
        {
            // Assume a clean exit
            Exit(0);
        }

        private void Exit(int code)
        {
            if (code == 0)
            {
                Settings.Default.Save();
            }

            Environment.Exit(code);
        }
    }
}
