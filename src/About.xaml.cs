using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
