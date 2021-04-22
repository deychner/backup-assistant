using BackupAssistant.Core;
using System;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        private delegate void ReportProgressCallback(int progress);
        private delegate void ReportStatusCallback(string status);
        private delegate void PreProcessCallback();
        private delegate void PostProcessCallback();

        public void PostProcess()
        {
            if (this.InvokeRequired)
            {
                var d = new PreProcessCallback(PostProcessInternal);
                this.Invoke(d);
            }
            else
            {
                PostProcessInternal();
            }
        }

        private void PostProcessInternal()
        {
            this.UI_Button_Backup.Enabled = true;
        }

        public void PreProcess()
        {
            if (this.InvokeRequired)
            {
                var d = new PreProcessCallback(PreProcessInternal);
                this.Invoke(d);
            }
            else
            {
                PreProcessInternal();
            }
        }

        private void PreProcessInternal()
        {
            this.UI_Button_Backup.Enabled = false;
        }

        public void ReportProgress(int progress)
        {
            if (this.InvokeRequired)
            {
                var d = new ReportProgressCallback(ReportProgressInternal);
                this.Invoke(d, new object[] { progress });
            }
            else
            {
                ReportProgressInternal(progress);
            }
        }

        private void ReportProgressInternal(int progress)
        {
            this.UI_ToolStripProgressBar.Value = progress;
        }

        public void ReportStatus(string status)
        {
            if (this.InvokeRequired)
            {
                var d = new ReportStatusCallback(ReportStatusInternal);
                this.Invoke(d, new object[] { status });
            }
            else
            {
                ReportStatusInternal(status);
            }
        }

        private void ReportStatusInternal(string status)
        {
            this.UI_ToolStripStatus.Text = status;
        }

        private void HandleException(Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
