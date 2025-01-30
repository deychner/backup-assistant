using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        internal async Task RunIncrementalBackupInternalAsync(CancellationToken token)
        {
            this.Progress = 0;

            // Get file list
            this.ProgressBarIsIndeterminate = true;
            this.Status = "Getting file information...";
            IDictionary<string, FileListing> files = await GetCombinedFileListAsync(this.Source, this.Destination, token);

            // Remove entries where BackupAction is None
            files = files
                .Where(f => f.Value.GetBackupAction() != BackupAction.None)
                .ToDictionary(e => e.Key, e => e.Value);

            // Get total size
            float processed = 0F;
            float totalSize =
                (from FileListing f in files.Values
                 select f.Size).Sum();

            // Process files
            this.ProgressBarIsIndeterminate = false;
            this.Status = $"Processing {files.Keys.Count} files...";

            var tasks = files.Keys.Select(async key =>
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                FileListing listing = files[key];
                string sourceFile = ExpandSourceFileName(key);
                string destinationFile = ExpandDestinationFileName(key);

                switch (listing.GetBackupAction())
                {
                    case BackupAction.Copy:
                        await SafeCopyFileAsync(sourceFile, destinationFile, false);
                        break;
                    case BackupAction.Overwrite:
                        await SafeCopyFileAsync(sourceFile, destinationFile, true);
                        break;
                    case BackupAction.Delete:
                        await SafeDeleteFileAsync(destinationFile);
                        break;
                    default:
                        // Do nothing
                        break;
                }

                // File was processed, so add it to the running total
                Extensions.Interlocked.Add(ref processed, listing.Size);

                // Handle edge case where all files eligible for backup are empty
                if (totalSize > 0)
                {
                    this.Progress = (int)(100F * (processed / totalSize));
                }
                else
                {
                    this.Progress = 100;
                }
            });

            await Task.WhenAll(tasks);

            this.Status = "Backup is complete.";
        }

        public async Task<IDictionary<string, FileListing>> GetCombinedFileListAsync(string sourceDirectory, string destinationDirectory, CancellationToken token)
        {
            ConcurrentDictionary<string, FileListing> files = new();

            if (this.FilterItems.Count > 0)
            {
                // Get files in root source directory
                this.Status = "Getting file information for source directory...";
                CollectFilesForSourceListing(sourceDirectory, files, token);

                // Get files in filtered source directories and all subdirectories
                var sourceTasks = this.FilterItems.Select(async f =>
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    // Each filter directory is a relative path
                    string d = GetFullFileName(f, sourceDirectory);
                    await SourceDirectorySearchAsync(d, files, token);
                });

                await Task.WhenAll(sourceTasks);

                // Get files in root destination directory
                this.Status = "Getting file information for destination directory...";
                CollectFilesForDestinationListing(destinationDirectory, files, token);

                // Get files in filtered destination directories and all subdirectories
                var destinationTasks = this.FilterItems.Select(async f =>
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    // Each filter directory is a relative path
                    string d = GetFullFileName(f, destinationDirectory);
                    await DestinationDirectorySearchAsync(d, files, token);
                });

                await Task.WhenAll(destinationTasks);
            }
            else
            {
                this.Status = "Getting file information for source directory...";
                await SourceDirectorySearchAsync(sourceDirectory, files, token);

                this.Status = "Getting file information for destination directory...";
                await DestinationDirectorySearchAsync(destinationDirectory, files, token);
            }

            return files;
        }

        private async Task SourceDirectorySearchAsync(string directory, ConcurrentDictionary<string, FileListing> fileList, CancellationToken token)
        {
            CollectFilesForSourceListing(directory, fileList, token);

            var tasks = SafeGetDirectories(directory).Select(async d =>
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                await SourceDirectorySearchAsync(d, fileList, token);
            });

            await Task.WhenAll(tasks);
        }

        private void CollectFilesForSourceListing(string directory, ConcurrentDictionary<string, FileListing> fileList, CancellationToken token)
        {
            var files = SafeGetFiles(directory);

            foreach (var f in files)
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                string fileName = ShrinkSourceFileName(f);
                IFileInfo? info = SafeGetFileInfo(f);

                if (info != null)
                {
                    fileList.AddOrUpdate(fileName, new FileListing
                    {
                        IsInSource = true,
                        SourceLastModified = info.LastWriteTime,
                        Size = info.Length
                    },
                    (key, existingListing) =>
                    {
                        existingListing.IsInSource = true;
                        existingListing.SourceLastModified = info.LastWriteTime;
                        return existingListing;
                    });
                }
            }
        }

        private async Task DestinationDirectorySearchAsync(string directory, ConcurrentDictionary<string, FileListing> fileList, CancellationToken token)
        {
            CollectFilesForDestinationListing(directory, fileList, token);

            var tasks = SafeGetDirectories(directory).Select(async d =>
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                await DestinationDirectorySearchAsync(d, fileList, token);
            });

            await Task.WhenAll(tasks);
        }

        private void CollectFilesForDestinationListing(string directory, ConcurrentDictionary<string, FileListing> fileList, CancellationToken token)
        {
            var files = SafeGetFiles(directory);

            foreach (var f in files)
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                string fileName = ShrinkDestinationFileName(f);
                IFileInfo? info = SafeGetFileInfo(f);

                if (info != null)
                {
                    fileList.AddOrUpdate(fileName, new FileListing
                    {
                        IsInDestination = true,
                        DestinationLastModified = info.LastWriteTime,
                        Size = info.Length
                    },
                    (key, existingListing) =>
                    {
                        existingListing.IsInDestination = true;
                        existingListing.DestinationLastModified = info.LastWriteTime;
                        return existingListing;
                    });
                }
            }
        }
    }
}
