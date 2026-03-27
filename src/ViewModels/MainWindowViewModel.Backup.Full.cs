using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Concurrent;
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
            ICollection<string> sourceFiles = await GetFileListAsync(this.Source, token);

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
            IEnumerable<Task> tasks = sourceFiles.Select(async (sourceFile) =>
            {
                // Wait for concurrency slot to become available
                await _concurrencyLimiter.WaitAsync(token);
                try
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    string destinationFile = sourceFile.Replace(this.Source, this.Destination);
                    await SafeCopyFileAsync(sourceFile, destinationFile, false);

                    // Update progress
                    _ = Interlocked.Increment(ref processed);
                    this.Progress = 100 * processed / sourceFiles.Count;
                }
                finally
                {
                    // Release the concurrency slot
                    _concurrencyLimiter.Release();
                }
            });

            await Task.WhenAll(tasks);

            this.Status = "Backup is complete.";
        }

        public async Task<ICollection<string>> GetFileListAsync(string rootDirectory, CancellationToken token)
        {
            ConcurrentBag<string> files = [];

            if (this.FilterItems.Count > 0)
            {
                // Get files in root directory
                GetFilesInDirectory(rootDirectory, files, token);

                // Get files in filtered directories and all subdirectories
                IEnumerable<Task> tasks = this.FilterItems.Select(async f =>
                {
                    // Wait for concurrency slot to become available
                    await _concurrencyLimiter.WaitAsync(token);
                    try
                    {
                        // Check for cancellation
                        token.ThrowIfCancellationRequested();

                        string d = GetFullFileName(f, rootDirectory);
                        await DirectorySearchAsync(d, files, token);
                    }
                    finally
                    {
                        // Release the concurrency slot
                        _concurrencyLimiter.Release();
                    }
                });

                await Task.WhenAll(tasks);
            }
            else
            {
                await DirectorySearchAsync(rootDirectory, files, token);
            }

            return [.. files];
        }

        private async Task DirectorySearchAsync(string directory, ConcurrentBag<string> fileList, CancellationToken token)
        {
            GetFilesInDirectory(directory, fileList, token);

            IEnumerable<Task> tasks = SafeEnumerateDirectories(directory).Select(async d =>
            {
                // Wait for concurrency slot to become available
                await _concurrencyLimiter.WaitAsync(token);
                try
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    await DirectorySearchAsync(d, fileList, token);
                }
                finally
                {
                    // Release the concurrency slot
                    _concurrencyLimiter.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private void GetFilesInDirectory(string directory, ConcurrentBag<string> fileList, CancellationToken token)
        {
            foreach (string f in SafeEnumerateFiles(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                fileList.Add(f);
            }
        }
    }
}
