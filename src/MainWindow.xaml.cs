using BackupAssistant.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace BackupAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = App.Current.Services.GetService<MainWindowViewModel>();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var window = new About
            {
                Owner = this
            };

            window.ShowDialog();
        }
    }
}
