using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace BackupAssistant.Core
{
    internal partial class BackupAgent
    {
        public void RunIncrementalBackup()
        {
            _caller.PreProcess();

            ValidateBackup();

            _caller.ReportProgress(0);
            _cancelOperation = false;

            // Get file list
            _caller.ReportStatus("Getting file information...");
            IDictionary<string, FileListing> files = GetCombinedFileList(_caller.SourcePath, _caller.DestinationPath);

            // Process files
            _caller.ReportStatus("Processing files...");

            // Get total size
            long processed = 0L;
            long totalSize =
                (from FileListing f in files.Values
                 where f.GetBackupAction() != BackupAction.None
                 select f.Size).Sum();

            foreach (string key in files.Keys)
            {
                // Check for cancellation
                if (_cancelOperation)
                {
                    break;
                }

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
                    _caller.ReportProgress((int)(100L * (processed / totalSize)));
                }
                else
                {
                    _caller.ReportProgress(100);
                }
            }

            if (_cancelOperation)
            {
                _caller.ReportStatus("Backup was canceled.");
            }
            else
            {
                _caller.ReportStatus("Backup is complete.");
            }

            _caller.PostProcess();
        }

        public IDictionary<string, FileListing> GetCombinedFileList(string sourceDirectory, string destinationDirectory)
        {
            Dictionary<string, FileListing> files = new Dictionary<string, FileListing>();

            if (_caller.Filters.Count > 0)
            {
                // Get files in root source directory
                _caller.ReportStatus("Getting file information for source directory...");
                CollectFilesForSourceListing(sourceDirectory, files);

                // Get files in filtered source directories and all subdirectories
                foreach (string f in _caller.Filters)
                {
                    // Each filter directory is a relative path
                    string d = GetFullFileName(f, sourceDirectory);
                    SourceDirectorySearch(d, files);
                }

                // Get files in root destination directory
                _caller.ReportStatus("Getting file information for destination directory...");
                CollectFilesForDestinationListing(destinationDirectory, files);

                // Get files in filtered source directories and all subdirectories
                foreach (string f in _caller.Filters)
                {
                    // Each filter directory is a relative path
                    string d = GetFullFileName(f, destinationDirectory);
                    DestinationDirectorySearch(d, files);
                }
            }
            else
            {
                _caller.ReportStatus("Getting file information for source directory...");
                SourceDirectorySearch(sourceDirectory, files);

                _caller.ReportStatus("Getting file information for destination directory...");
                DestinationDirectorySearch(destinationDirectory, files);
            }

            return files;
        }

        private void SourceDirectorySearch(string directory, IDictionary<string, FileListing> fileList)
        {
            CollectFilesForSourceListing(directory, fileList);

            foreach (string d in SafeGetDirectories(directory))
            {
                SourceDirectorySearch(d, fileList);
            }
        }

        private void CollectFilesForSourceListing(string directory, IDictionary<string, FileListing> fileList)
        {
            foreach (string f in SafeGetFiles(directory))
            {
                string fileName = ShrinkSourceFileName(f);
                IFileInfo info = SafeGetFileInfo(f);

                if (info != null)
                {
                    if (fileList.ContainsKey(fileName))
                    {
                        fileList[fileName].IsInSource = true;
                        fileList[fileName].SourceLastModified = info.LastWriteTime;
                    }
                    else
                    {
                        FileListing listing = new FileListing
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

        private void DestinationDirectorySearch(string directory, IDictionary<string, FileListing> fileList)
        {
            CollectFilesForDestinationListing(directory, fileList);

            foreach (string d in SafeGetDirectories(directory))
            {
                DestinationDirectorySearch(d, fileList);
            }
        }

        private void CollectFilesForDestinationListing(string directory, IDictionary<string, FileListing> fileList)
        {
            foreach (string f in SafeGetFiles(directory))
            {
                string fileName = ShrinkDestinationFileName(f);
                IFileInfo info = SafeGetFileInfo(f);

                if (info != null)
                {
                    if (fileList.ContainsKey(fileName))
                    {
                        fileList[fileName].IsInDestination = true;
                        fileList[fileName].DestinationLastModified = info.LastWriteTime;
                    }
                    else
                    {
                        FileListing listing = new FileListing
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
