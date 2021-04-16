using BackupAssistant.Core;
using BackupAssistant.Properties;
using System.Windows.Forms;

namespace BackupAssistant
{
    public partial class UI : Form, IBackupStarter
    {
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
    }
}
