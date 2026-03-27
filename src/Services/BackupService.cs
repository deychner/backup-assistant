using BackupAssistant.DataModels;
using BackupAssistant.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackupAssistant.Services
{
    public class BackupService(IFileSystem fileSystem, ILogService logService) : IBackupService
    {
        private const string FilePathAbbreviation = "...";

        // Using 2x processor cores as a conservative limit for I/O bound operations
        private readonly SemaphoreSlim _concurrencyLimiter = new(Environment.ProcessorCount * 2, Environment.ProcessorCount * 2);

        private readonly IFileSystem _fileSystem = fileSystem;
        private readonly ILogService _logService = logService;

        public async Task RunFullBackupAsync(
            string source,
            string destination,
            ICollection<string> filterItems,
            IProgress<BackupProgress> progress,
            CancellationToken token)
        {
            progress?.Report(new BackupProgress { Progress = 0, IsIndeterminate = true, Status = "Getting source file list..." });

            // Get file list
            ICollection<string> sourceFiles = await GetFileListAsync(source, filterItems, token);

            // Delete destination directory
            progress?.Report(new BackupProgress { Status = "Deleting destination directory..." });
            if (_fileSystem.Directory.Exists(destination))
            {
                _fileSystem.Directory.Delete(destination, true);
            }

            // Copy files
            progress?.Report(new BackupProgress
            {
                IsIndeterminate = false,
                Status = $"Copying {sourceFiles.Count} files..."
            });

            int processed = 0;
            IEnumerable<Task> tasks = sourceFiles.Select(async (sourceFile) =>
            {
                // Wait for concurrency slot to become available
                await _concurrencyLimiter.WaitAsync(token);
                try
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    string destinationFile = sourceFile.Replace(source, destination);
                    await SafeCopyFileAsync(sourceFile, destinationFile, false);

                    // Update progress
                    _ = System.Threading.Interlocked.Increment(ref processed);
                    int currentProgress = 100 * processed / sourceFiles.Count;
                    progress?.Report(new BackupProgress { Progress = currentProgress });
                }
                finally
                {
                    // Release the concurrency slot
                    _concurrencyLimiter.Release();
                }
            });

            await Task.WhenAll(tasks);

            progress?.Report(new BackupProgress { Status = "Backup is complete." });
        }

        public async Task RunIncrementalBackupAsync(
            string source,
            string destination,
            ICollection<string> filterItems,
            IProgress<BackupProgress> progress,
            CancellationToken token)
        {
            progress?.Report(new BackupProgress { Progress = 0, IsIndeterminate = true, Status = "Getting file information..." });

            // Get file list
            IDictionary<string, FileListing> files = await GetCombinedFileListAsync(source, destination, filterItems, token);

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
            progress?.Report(new BackupProgress
            {
                IsIndeterminate = false,
                Status = $"Processing {files.Keys.Count} files..."
            });

            IEnumerable<Task> tasks = files.Keys.Select(async key =>
            {
                // Wait for concurrency slot to become available
                await _concurrencyLimiter.WaitAsync(token);
                try
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    FileListing listing = files[key];
                    string sourceFile = ExpandSourceFileName(key, source);
                    string destinationFile = ExpandDestinationFileName(key, destination);

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
                    _ = Extensions.Interlocked.Add(ref processed, listing.Size);

                    // Handle edge case where all files eligible for backup are empty
                    int currentProgress = totalSize > 0 ? (int)(100F * (processed / totalSize)) : 100;
                    progress?.Report(new BackupProgress { Progress = currentProgress });
                }
                finally
                {
                    // Release the concurrency slot
                    _concurrencyLimiter.Release();
                }
            });

            await Task.WhenAll(tasks);

            progress?.Report(new BackupProgress { Status = "Backup is complete." });
        }

        internal async Task<ICollection<string>> GetFileListAsync(string rootDirectory, ICollection<string> filterItems, CancellationToken token)
        {
            ConcurrentBag<string> files = [];

            if (filterItems.Count > 0)
            {
                // Get files in root directory
                GetFilesInDirectory(rootDirectory, files, token);

                // Get files in filtered directories and all subdirectories
                IEnumerable<Task> tasks = filterItems.Select(async f =>
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

        internal async Task<IDictionary<string, FileListing>> GetCombinedFileListAsync(string sourceDirectory, string destinationDirectory, ICollection<string> filterItems, CancellationToken token)
        {
            ConcurrentDictionary<string, FileListing> files = new();

            if (filterItems.Count > 0)
            {
                // Get files in root source directory
                CollectFilesForSourceListing(sourceDirectory, files, sourceDirectory, token);

                // Get files in filtered source directories and all subdirectories
                IEnumerable<Task> sourceTasks = filterItems.Select(async f =>
                {
                    // Wait for concurrency slot to become available
                    await _concurrencyLimiter.WaitAsync(token);
                    try
                    {
                        // Check for cancellation
                        token.ThrowIfCancellationRequested();

                        // Each filter directory is a relative path
                        string d = GetFullFileName(f, sourceDirectory);
                        await SourceDirectorySearchAsync(d, files, sourceDirectory, token);
                    }
                    finally
                    {
                        // Release the concurrency slot
                        _concurrencyLimiter.Release();
                    }
                });

                await Task.WhenAll(sourceTasks);

                // Get files in root destination directory
                CollectFilesForDestinationListing(destinationDirectory, files, destinationDirectory, token);

                // Get files in filtered destination directories and all subdirectories
                IEnumerable<Task> destinationTasks = filterItems.Select(async f =>
                {
                    // Wait for concurrency slot to become available
                    await _concurrencyLimiter.WaitAsync(token);
                    try
                    {
                        // Check for cancellation
                        token.ThrowIfCancellationRequested();

                        // Each filter directory is a relative path
                        string d = GetFullFileName(f, destinationDirectory);
                        await DestinationDirectorySearchAsync(d, files, destinationDirectory, token);
                    }
                    finally
                    {
                        // Release the concurrency slot
                        _concurrencyLimiter.Release();
                    }
                });

                await Task.WhenAll(destinationTasks);
            }
            else
            {
                await SourceDirectorySearchAsync(sourceDirectory, files, sourceDirectory, token);
                await DestinationDirectorySearchAsync(destinationDirectory, files, destinationDirectory, token);
            }

            return files;
        }

        #region Private Methods

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

        private async Task SourceDirectorySearchAsync(string directory, ConcurrentDictionary<string, FileListing> fileList, string sourceRoot, CancellationToken token)
        {
            CollectFilesForSourceListing(directory, fileList, sourceRoot, token);

            IEnumerable<Task> tasks = SafeEnumerateDirectories(directory).Select(async d =>
            {
                // Wait for concurrency slot to become available
                await _concurrencyLimiter.WaitAsync(token);
                try
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    await SourceDirectorySearchAsync(d, fileList, sourceRoot, token);
                }
                finally
                {
                    // Always release the concurrency slot
                    _concurrencyLimiter.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private void CollectFilesForSourceListing(string directory, ConcurrentDictionary<string, FileListing> fileList, string sourceRoot, CancellationToken token)
        {
            foreach (string f in SafeEnumerateFiles(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                string fileName = ShrinkSourceFileName(f, sourceRoot);
                IFileInfo? info = SafeGetFileInfo(f);

                if (info != null)
                {
                    _ = fileList.AddOrUpdate(fileName, new FileListing
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

        private async Task DestinationDirectorySearchAsync(string directory, ConcurrentDictionary<string, FileListing> fileList, string destinationRoot, CancellationToken token)
        {
            CollectFilesForDestinationListing(directory, fileList, destinationRoot, token);

            IEnumerable<Task> tasks = SafeEnumerateDirectories(directory).Select(async d =>
            {
                // Wait for concurrency slot to become available
                await _concurrencyLimiter.WaitAsync(token);
                try
                {
                    // Check for cancellation
                    token.ThrowIfCancellationRequested();

                    await DestinationDirectorySearchAsync(d, fileList, destinationRoot, token);
                }
                finally
                {
                    // Always release the concurrency slot
                    _concurrencyLimiter.Release();
                }
            });

            await Task.WhenAll(tasks);
        }

        private void CollectFilesForDestinationListing(string directory, ConcurrentDictionary<string, FileListing> fileList, string destinationRoot, CancellationToken token)
        {
            foreach (string f in SafeEnumerateFiles(directory))
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                string fileName = ShrinkDestinationFileName(f, destinationRoot);
                IFileInfo? info = SafeGetFileInfo(f);

                if (info != null)
                {
                    _ = fileList.AddOrUpdate(fileName, new FileListing
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

        #endregion

        #region Safety

        private IFileInfo? SafeGetFileInfo(string file)
        {
            try
            {
                return _fileSystem.FileInfo.New(file);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Failed to get file information for file '{file}', Exception: {e.Message}");

                return null;
            }
        }

        private IEnumerable<string> SafeEnumerateFiles(string directory)
        {
            try
            {
                return _fileSystem.Directory.EnumerateFiles(directory);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Failed to get files in directory '{directory}', Exception: {e.Message}");
                return [];
            }
        }

        private IEnumerable<string> SafeEnumerateDirectories(string directory)
        {
            try
            {
                return _fileSystem.Directory.EnumerateDirectories(directory);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Failed to get directories in directory '{directory}', Exception: {e.Message}");
                return [];
            }
        }

        private void EnsureDirectoryPathExists(string path)
        {
            string? directory = _fileSystem.Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !_fileSystem.Directory.Exists(directory))
            {
                _ = _fileSystem.Directory.CreateDirectory(directory);
            }
        }

        private async Task SafeCopyFileAsync(string sourceFileName, string destinationFileName, bool overwrite)
        {
            try
            {
                EnsureDirectoryPathExists(destinationFileName);

                await Task.Run(() => _fileSystem.File.Copy(sourceFileName, destinationFileName, overwrite));
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Copy file failed for file '{sourceFileName}', Exception: {e.Message}");
            }
        }

        private async Task SafeDeleteFileAsync(string file)
        {
            try
            {
                await Task.Run(() => _fileSystem.File.Delete(file));
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Delete file failed for file '{file}', Exception: {e.Message}");
            }
        }

        #endregion

        #region Compression

        private static string ShrinkSourceFileName(string fileName, string source)
        {
            return GetAbbreviatedFileName(fileName, source);
        }

        private static string ShrinkDestinationFileName(string fileName, string destination)
        {
            return GetAbbreviatedFileName(fileName, destination);
        }

        private static string ExpandSourceFileName(string fileName, string source)
        {
            return GetFullFileName(fileName, source);
        }

        private static string ExpandDestinationFileName(string fileName, string destination)
        {
            return GetFullFileName(fileName, destination);
        }

        private static string GetFullFileName(string abbreviatedFileName, string prefix)
        {
            return abbreviatedFileName.ReplaceFirst(FilePathAbbreviation, prefix).Replace(@"\\", @"\");
        }

        private static string GetAbbreviatedFileName(string fileName, string prefix)
        {
            // Ensure all shortened file names start with "...\"
            return prefix.EndsWith('\\')
                ? fileName.ReplaceFirst(prefix, $@"{FilePathAbbreviation}\")
                : fileName.ReplaceFirst(prefix, FilePathAbbreviation);
        }

        #endregion
    }
}