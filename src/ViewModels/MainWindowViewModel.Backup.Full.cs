using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Threading;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        internal void RunFullBackupInternal(CancellationToken token)
        {
            this.Progress = 0;

            // Get file list
            this.Status = "Getting source file list...";
            IList<string> sourceFiles = GetFileList(this.Source, token);

            // Delete destination directory
            this.Status = "Deleting destination directory...";
            if (_fileSystem.Directory.Exists(this.Destination))
            {
                _fileSystem.Directory.Delete(this.Destination, true);
            }

            // Copy files
            this.Status = "Copying files...";
            for (int i = 0; i < sourceFiles.Count; i++)
            {
                // Check for cancellation
                token.ThrowIfCancellationRequested();

                string destinationFile = sourceFiles[i].Replace(this.Source, this.Destination);
                SafeCopyFile(sourceFiles[i], destinationFile);

                this.Progress = (100 * (i + 1) / sourceFiles.Count);
            }

            this.Status = "Operation complete!";
        }

        private IList<string> GetFileList(string rootDirectory, CancellationToken token)
        {
            List<string> files = new();

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
