using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {

        internal void RunIncrementalBackupInternal(CancellationToken token)
        {
            this.Progress = 0;

            // Get file list
            this.ProgressBarIsIndeterminate = true;
            this.Status = "Getting file information...";
            IDictionary<string, FileListing> files = GetCombinedFileList(this.Source, this.Destination, token);

            // Get total size
            float processed = 0F;
            float totalSize =
                (from FileListing f in files.Values
                 where f.GetBackupAction() != BackupAction.None
                 select f.Size).Sum();
            float totalFiles =
                (from FileListing f in files.Values
                 where f.GetBackupAction() != BackupAction.None
                 select 1).Count();

            // Process files
            this.ProgressBarIsIndeterminate = false;
            this.Status = $"Processing {totalFiles} files...";
            foreach (string key in files.Keys)
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                FileListing listing = files[key];
                string sourceFile = ExpandSourceFileName(key);
                string destinationFile = ExpandDestinationFileName(key);

                switch (listing.GetBackupAction())
                {
                    case BackupAction.None:
                        // Do nothing
                        break;
                    case BackupAction.Copy:
                        SafeCopyFile(sourceFile, destinationFile, false);
                        processed += listing.Size;
                        break;
                    case BackupAction.Overwrite:
                        SafeCopyFile(sourceFile, destinationFile, true);
                        processed += listing.Size;
                        break;
                    case BackupAction.Delete:
                        SafeDeleteFile(destinationFile);
                        processed += listing.Size;
                        break;
                    default:
                        // Do nothing
                        break;
                }

                // Handle edge case where all files eligible for backup are empty
                if (totalSize > 0)
                {
                    this.Progress = (int)(100F * (processed / totalSize));
                }
                else
                {
                    this.Progress = 100;
                }
            }

            this.Status = "Backup is complete.";
        }

        public IDictionary<string, FileListing> GetCombinedFileList(string sourceDirectory, string destinationDirectory, CancellationToken token)
        {
            Dictionary<string, FileListing> files = new();

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
                    if (fileList.ContainsKey(fileName))
                    {
                        fileList[fileName].IsInSource = true;
                        fileList[fileName].SourceLastModified = info.LastWriteTime;
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
                    if (fileList.ContainsKey(fileName))
                    {
                        fileList[fileName].IsInDestination = true;
                        fileList[fileName].DestinationLastModified = info.LastWriteTime;
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
