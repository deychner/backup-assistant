using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
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
            IDictionary<string, FileListing> files = GetCombinedFileList(this.Source, this.Destination, token);

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

        public IDictionary<string, FileListing> GetCombinedFileList(string sourceDirectory, string destinationDirectory, CancellationToken token)
        {
            Dictionary<string, FileListing> files = [];

            if (this.FilterItems.Count > 0)
            {
                // Get files in root source directory
                this.Status = "Getting file information for source directory...";
                CollectFilesForSourceListing(sourceDirectory, files);

                // Get files in filtered source directories and all subdirectories
                foreach (string f in this.FilterItems)
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    // Each filter directory is a relative path
                    string d = GetFullFileName(f, sourceDirectory);
                    SourceDirectorySearch(d, files, token);
                }

                // Get files in root destination directory
                this.Status = "Getting file information for destination directory...";
                CollectFilesForDestinationListing(destinationDirectory, files);

                // Get files in filtered source directories and all subdirectories
                foreach (string f in this.FilterItems)
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    // Each filter directory is a relative path
                    string d = GetFullFileName(f, destinationDirectory);
                    DestinationDirectorySearch(d, files, token);
                }
            }
            else
            {
                this.Status = "Getting file information for source directory...";
                SourceDirectorySearch(sourceDirectory, files, token);

                this.Status = "Getting file information for destination directory...";
                DestinationDirectorySearch(destinationDirectory, files, token);
            }

            return files;
        }

        private void SourceDirectorySearch(string directory, IDictionary<string, FileListing> fileList, CancellationToken token)
        {
            CollectFilesForSourceListing(directory, fileList);

            foreach (string d in SafeGetDirectories(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                SourceDirectorySearch(d, fileList, token);
            }
        }

        private void CollectFilesForSourceListing(string directory, IDictionary<string, FileListing> fileList)
        {
            foreach (string f in SafeGetFiles(directory))
            {
                string fileName = ShrinkSourceFileName(f);
                IFileInfo? info = SafeGetFileInfo(f);

                if (info != null)
                {
                    if (fileList.TryGetValue(fileName, out FileListing? existingListing))
                    {
                        existingListing.IsInSource = true;
                        existingListing.SourceLastModified = info.LastWriteTime;
                    }
                    else
                    {
                        FileListing listing = new()
                        {
                            IsInSource = true,
                            SourceLastModified = info.LastWriteTime,
                            Size = info.Length
                        };

                        fileList.Add(fileName, listing);
                    }
                }
            }
        }

        private void DestinationDirectorySearch(string directory, IDictionary<string, FileListing> fileList, CancellationToken token)
        {
            CollectFilesForDestinationListing(directory, fileList);

            foreach (string d in SafeGetDirectories(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                DestinationDirectorySearch(d, fileList, token);
            }
        }

        private void CollectFilesForDestinationListing(string directory, IDictionary<string, FileListing> fileList)
        {
            foreach (string f in SafeGetFiles(directory))
            {
                string fileName = ShrinkDestinationFileName(f);
                IFileInfo? info = SafeGetFileInfo(f);

                if (info != null)
                {
                    if (fileList.TryGetValue(fileName, out FileListing? existingListing))
                    {
                        existingListing.IsInDestination = true;
                        existingListing.DestinationLastModified = info.LastWriteTime;
                    }
                    else
                    {
                        FileListing listing = new()
                        {
                            IsInDestination = true,
                            DestinationLastModified = info.LastWriteTime,
                            Size = info.Length
                        };

                        fileList.Add(fileName, listing);
                    }
                }
            }
        }
    }
}
