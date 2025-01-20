using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        internal async Task RunFullBackupInternalAsync(CancellationToken token)
        {
            this.Progress = 0;

            // Get file list
            this.ProgressBarIsIndeterminate = true;
            this.Status = "Getting source file list...";
            IList<string> sourceFiles = GetFileList(this.Source, token);

            // Delete destination directory
            this.Status = "Deleting destination directory...";
            if (_fileSystem.Directory.Exists(this.Destination))
            {
                _fileSystem.Directory.Delete(this.Destination, true);
            }

            // Copy files
            this.ProgressBarIsIndeterminate = false;
            this.Status = $"Copying {sourceFiles.Count} files...";

            int processed = 0;
            var tasks = sourceFiles.Select(async (sourceFile) =>
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                string destinationFile = sourceFile.Replace(this.Source, this.Destination);
                await SafeCopyFileAsync(sourceFile, destinationFile, false);

                // Update progress
                Interlocked.Increment(ref processed);
                this.Progress = 100 * (processed) / sourceFiles.Count;
            });

            await Task.WhenAll(tasks);

            this.Status = "Backup is complete.";
        }

        public IList<string> GetFileList(string rootDirectory, CancellationToken token)
        {
            List<string> files = [];

            if (this.FilterItems.Count > 0)
            {
                // Get files in root directory
                GetFilesInDirectory(rootDirectory, files, token);

                // Get files in filtered directories and all subdirectories
                foreach (string f in this.FilterItems)
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    string d = GetFullFileName(f, rootDirectory);
                    DirectorySearch(d, files, token);
                }
            }
            else
            {
                DirectorySearch(rootDirectory, files, token);
            }

            return files;
        }

        private void DirectorySearch(string directory, IList<string> fileList, CancellationToken token)
        {
            GetFilesInDirectory(directory, fileList, token);

            foreach (string d in SafeGetDirectories(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                DirectorySearch(d, fileList, token);
            }
        }

        private void GetFilesInDirectory(string directory, IList<string> fileList, CancellationToken token)
        {
            foreach (string f in SafeGetFiles(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                fileList.Add(f);
            }
        }
    }
}
