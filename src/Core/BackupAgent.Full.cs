using System.Collections.Generic;

namespace BackupAssistant.Core
{
    internal partial class BackupAgent
    {
        public void RunFullBackup()
        {
            _caller.PreProcess();

            ValidateBackup();

            _caller.ReportProgress(0);
            _cancelOperation = false;

            // Get file list
            _caller.ReportStatus("Getting source file list...");
            List<string> sourceFiles = (List<string>)GetFileList(_caller.SourcePath);

            // Delete destination directory
            _caller.ReportStatus("Deleting destination directory...");
            if (_fileSystem.Directory.Exists(_caller.DestinationPath))
            {
                _fileSystem.Directory.Delete(_caller.DestinationPath, true);
            }

            // Copy files
            _caller.ReportStatus("Copying files...");
            for (int i = 0; i < sourceFiles.Count - 1; i++)
            {
                // Check for cancellation
                if (_cancelOperation)
                {
                    break;
                }

                string destinationFile = sourceFiles[i].Replace(_caller.SourcePath, _caller.DestinationPath);
                SafeCopyFile(sourceFiles[i], destinationFile);

                _caller.ReportProgress(100 * (i + 1) / sourceFiles.Count);
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

        public ICollection<string> GetFileList(string rootDirectory)
        {
            List<string> files = new List<string>();

            if (_caller.Filters.Count > 0)
            {
                // Get files in root directory
                GetFilesInDirectory(rootDirectory, files);

                // Get files in filtered directories and all subdirectories
                foreach (string f in _caller.Filters)
                {
                    string d = GetFullFileName(f, rootDirectory);
                    DirectorySearch(d, files);
                }
            }
            else
            {
                DirectorySearch(rootDirectory, files);
            }

            return files;
        }

        private void DirectorySearch(string directory, ICollection<string> fileList)
        {
            GetFilesInDirectory(directory, fileList);

            foreach (string d in SafeGetDirectories(directory))
            {
                DirectorySearch(d, fileList);
            }
        }

        private void GetFilesInDirectory(string directory, ICollection<string> fileList)
        {
            foreach (string f in SafeGetFiles(directory))
            {
                fileList.Add(f);
            }
        }
    }
}
