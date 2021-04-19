using BackupAssistant.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace BackupAssistant.Modals
{
    public partial class FilterSelection : Form
    {
        public FilterSelection(IBackupStarter caller)
        {
            InitializeComponent();

            // Populate list
            if (!string.IsNullOrEmpty(caller.SourcePath))
            {
                string[] directoriesToFilter = new string[0];

                try
                {
                    directoriesToFilter = Directory.GetDirectories(caller.SourcePath);
                }
                catch
                {
                    // Do nothing
                }

                foreach (string d in directoriesToFilter)
                {
                    string shortName = d.Replace(caller.SourcePath, "...");
                    this.FilterSelection_CheckedListBox_Filters.Items.Add(shortName, caller.Filters.Contains(shortName));
                }
            }
        }

        public IList<string> GetFilterList()
        {
            List<string> filters = new List<string>();

            foreach (object d in this.FilterSelection_CheckedListBox_Filters.CheckedItems)
            {
                filters.Add(d.ToString());
            }

            return filters;
        }
    }
}
