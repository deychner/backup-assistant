using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for FilterSelection.xaml
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class FilterSelection : Window
    {
        public FilterSelection()
        {
            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
