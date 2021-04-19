using BackupAssistant.Core;
using BackupAssistant.Modals;
using BackupAssistant.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
        private List<string> _filterList = null;
        public ReadOnlyCollection<string> Filters
        {
            get { return _filterList.AsReadOnly(); }
        }

        private void UI_Button_Source_Click(object sender, EventArgs e)
        {
            string directory = GetFolderFromUser();

            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                this.UI_TextBox_Source.Text = directory;
                Settings.Default.Source = directory;
            }
        }

        private void UI_Button_Destination_Click(object sender, EventArgs e)
        {
            string directory = GetFolderFromUser();

            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                this.UI_TextBox_Destination.Text = directory;
                Settings.Default.Destination = directory;
            }
        }

        private void UI_Button_Filter_Click(object sender, EventArgs e)
        {
            using (FilterSelection dialog = new FilterSelection(this))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // Update internal collection
                    _filterList = (List<string>)dialog.GetFilterList();

                    // Update settings
                    Settings.Default.Filters.Clear();
                    Settings.Default.Filters.AddRange(_filterList.ToArray());
                }
            }

            UpdateFilterIcon();
        }

        private void UI_TextBox_Source_TextChanged(object sender, EventArgs e)
        {
            this.UI_Button_Filter.Enabled = !string.IsNullOrEmpty(this.UI_TextBox_Source.Text);
        }

        private void UpdateFilterIcon()
        {
            if (Settings.Default.Filters.Count == 0)
            {
                this.UI_Button_Filter.Image = Resources.filter;
            }
            else
            {
                this.UI_Button_Filter.Image = Resources.filter_apply;
            }
        }

        private static string GetFolderFromUser()
        {
            using FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                SelectedPath = "C:\\",
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
