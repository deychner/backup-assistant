using Microsoft.UI.Xaml;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public sealed partial class About : Window
    {
        public About()
        {
            this.InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(null);
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
