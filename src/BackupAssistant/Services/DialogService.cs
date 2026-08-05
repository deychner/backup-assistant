using BackupAssistant.ViewModels;
using BackupAssistant.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BackupAssistant.Services
{
    /// <summary>
    /// WinUI implementation of <see cref="IDialogService"/>.
    /// <para>
    /// WinUI has no modal <c>Window.ShowDialog()</c>, so dialogs are <see cref="ContentDialog"/>
    /// instances hosted in the main window's <see cref="XamlRoot"/>. The window is resolved
    /// through a factory to avoid a circular dependency with the view models.
    /// </para>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DialogService(Func<Window> windowFactory, Func<AboutViewModel> aboutViewModelFactory) : IDialogService
    {
        public async Task<string?> ShowFolderPickerAsync()
        {
            Window window = windowFactory();

            // The Windows App SDK folder picker cannot be pointed at an arbitrary path; it takes a
            // well-known location and the shell then remembers where the user browsed to last.
            FolderPicker picker = new(window.AppWindow.Id)
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                CommitButtonText = "Select folder"
            };

            PickFolderResult? result = await picker.PickSingleFolderAsync();

            return result?.Path;
        }

        public async Task<bool> ShowFilterSelectionDialogAsync(FilterSelectionViewModel viewModel)
        {
            FilterSelectionDialog dialog = new(viewModel)
            {
                XamlRoot = windowFactory().Content.XamlRoot
            };

            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }

        public async Task ShowAboutDialogAsync()
        {
            AboutDialog dialog = new(aboutViewModelFactory())
            {
                XamlRoot = windowFactory().Content.XamlRoot
            };

            _ = await dialog.ShowAsync();
        }
    }
}
