using System;
using System.Windows.Forms;

namespace BackupAssistant.Modals
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            this.About_Label_ApplicationName.Text = Application.ProductName;
            this.About_Label_Version.Text = "Version " + Application.ProductVersion;
        }

        private void About_Button_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
