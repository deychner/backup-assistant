using BackupAssistant.Core;
using BackupAssistant.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        public UI()
        {
            InitializeComponent();

            Application.ApplicationExit += new EventHandler(this.Exit);

            if (!EventLog.SourceExists("Backup Assistant"))
            {
                HandleException(new NotSupportedException("You must establish an event log for this application."));
                Exit(1);
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
    }
}
