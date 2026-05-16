using BackupAssistant.ViewModels;
using Microsoft.UI.Xaml;
using Windows.Storage.Pickers;
using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace BackupAssistant.Services
{
    [ExcludeFromCodeCoverage]
    public class DialogService : IDialogService
    {
        public async (bool?, string) ShowOpenFolderDialog(string selectedPath = "")
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            // Initialize the folder picker window
            var window = ((App)Application.Current).m_window;
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WinRT.Interop.WindowNative.GetWindowHandle(window));

            if (!string.IsNullOrEmpty(selectedPath) && Directory.Exists(selectedPath))
            {
                try
                {
                    folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
                }
                catch
                {
                    // If suggested path doesn't work, just use default
                }
            }

            var folder_picked = await folderPicker.PickSingleFolderAsync();
            if (folder_picked != null)
            {
                return (true, folder_picked.Path);
            }

            return (false, "");
        }

        public bool? ShowDialog<T>(IDialogViewModel viewModel) where T : new()
        {
            // This method is no longer used in WinUI 3 approach - filters are shown inline
            // Keeping for backward compatibility but implementation is minimal
            return null;
        }
    }
}

