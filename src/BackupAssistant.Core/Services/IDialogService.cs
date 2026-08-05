using BackupAssistant.ViewModels;
using System.Threading.Tasks;

namespace BackupAssistant.Services
{
    /// <summary>
    /// Abstracts the modal interactions a view model needs. Keeping this interface free of
    /// UI types is what allows the view models to be unit tested without a XAML runtime.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Asks the user to pick a folder.
        /// </summary>
        /// <returns>The selected folder path, or <see langword="null"/> if the user cancelled.</returns>
        Task<string?> ShowFolderPickerAsync();

        /// <summary>
        /// Shows the folder selection dialog.
        /// </summary>
        /// <returns><see langword="true"/> if the user applied their changes.</returns>
        Task<bool> ShowFilterSelectionDialogAsync(FilterSelectionViewModel viewModel);

        /// <summary>
        /// Shows the about dialog.
        /// </summary>
        Task ShowAboutDialogAsync();
    }
}
