using BackupAssistant.Core;
using BackupAssistant.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
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
        private StringBuilder _logMessage = null;

        private List<string> _filterList = null;
        public ReadOnlyCollection<string> Filters
        {
            get { return _filterList.AsReadOnly(); }
        }

        private delegate void UpdateProcessCallback(int progress);
        private delegate void UpdateStatusCallback(string status);
        private delegate void PreProcessCallback();
        private delegate void PostProcessCallback();

        public UI()
        {
            InitializeComponent();

            if (!EventLog.SourceExists("Backup Assistant"))
            {
                HandleException(new NotSupportedException("You must establish an event log for this application."));
                Environment.Exit(1);
            }

            _backupAgent = new BackupAgent(this);
            _logMessage = new StringBuilder();
            _filterList = new List<string>();

            this.UI_ToolStripStatus.Text = string.Empty;

            LoadSettings();
        }

        private void LoadSettings()
        {
            if (string.IsNullOrEmpty(Settings.Default.Source))
            {
                // Default source
                this.UI_TextBox_Source.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                Settings.Default.Source = this.UI_TextBox_Source.Text;
            }
            else
            {
                // Load source
                this.UI_TextBox_Source.Text = Settings.Default.Source;
            }

            if (!string.IsNullOrEmpty(Settings.Default.Destination))
            {
                // Load destination
                this.UI_TextBox_Destination.Text = Settings.Default.Destination;
            }

            // Load backup type
            this.UI_ComboBox_Type.SelectedIndex = Settings.Default.BackupType;

            // Initialize filters
            Settings.Default.Filters ??= new System.Collections.Specialized.StringCollection();

            // Load filters
            foreach (string f in Settings.Default.Filters)
            {
                _filterList.Add(f);
            }

            UpdateFilterIcon();
        }

        private void UpdateFilterIcon()
        {
            if (Settings.Default.Filters.Count > 0)
            {
                this.UI_Button_Filter.Image = Resources.filter;
            }
            else
            {
                this.UI_Button_Filter.Image = Resources.filter_apply;
            }
        }

        public string SourcePath => throw new System.NotImplementedException();

        public string DestinationPath => throw new System.NotImplementedException();

        public void AddToLogEntry(string message)
        {
            throw new System.NotImplementedException();
        }

        public void PostProcess()
        {
            throw new System.NotImplementedException();
        }

        public void PreProcess()
        {
            throw new System.NotImplementedException();
        }

        public void ReportProgress(int progress)
        {
            throw new System.NotImplementedException();
        }

        public void ReportStatus(string status)
        {
            throw new System.NotImplementedException();
        }

        private void HandleException(Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //private async void button1_Click(object sender, System.EventArgs e)
        //{
        //    button1.Enabled = false;
        //    await Task.Run(() => BackupAgent.RunFullBackup());
        //    button1.Enabled = true;
        //}
    }
}
