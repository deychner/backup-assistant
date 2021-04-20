using System.Collections.ObjectModel;
using System.IO.Abstractions;

namespace BackupAssistant.Core
{
    internal partial class BackupAgent
    {
        public IFileInfo SafeGetFileInfo(string file)
        {
            try
            {
                return _fileSystem.FileInfo.FromFileName(file);
            }
            catch
            {
                _caller.AddToLogEntry($"Failed to get file information for file '{file}'.");

                return null;
            }
        }

        public ReadOnlyCollection<string> SafeGetFiles(string directory)
        {
            try
            {
                var files = _fileSystem.Directory.GetFiles(directory);
                return new ReadOnlyCollection<string>(files);
            }
            catch
            {
                _caller.AddToLogEntry($"Failed to get files in directory '{directory}'.");

                return new ReadOnlyCollection<string>(new string[] { });
            }
        }

        public ReadOnlyCollection<string> SafeGetDirectories(string directory)
        {
            try
            {
                var directories = _fileSystem.Directory.GetDirectories(directory);
                return new ReadOnlyCollection<string>(directories);
            }
            catch
            {
                _caller.AddToLogEntry($"Failed to get directories in directory '{directory}'.");

                return new ReadOnlyCollection<string>(new string[] { });
            }
        }

        public void EnsureDirectoryPathExists(string fileName)
        {
            string[] directories = fileName.Split('\\');
            string bottom = string.Join('\\', directories, 0, directories.Length - 1);

            // If the destination's directory structure does not exist, create it
            if (!_fileSystem.Directory.Exists(bottom))
            {
                for (int i = 1; i < directories.Length; i++)
                {
                    string path = string.Join('\\', directories, 0, i);

                    if (!_fileSystem.Directory.Exists(path))
                    {
                        _fileSystem.Directory.CreateDirectory(path);
                    }
                }
            }
        }

        public void SafeCopyFile(string sourceFileName, string destinationFileName)
        {
            SafeCopyFile(sourceFileName, destinationFileName, false);
        }

        public void SafeCopyFile(string sourceFileName, string destinationFileName, bool overwrite)
        {
            try
            {
                EnsureDirectoryPathExists(destinationFileName);

                _fileSystem.File.Copy(sourceFileName, destinationFileName, overwrite);
            }
            catch
            {
                _caller.AddToLogEntry($"Copy file failed for file '{sourceFileName}'.");
            }
        }

        public void SafeDeleteFile(string file)
        {
            try
            {
                _fileSystem.File.Delete(file);
            }
            catch
            {
                _caller.AddToLogEntry($"Delete file failed for file '{file}'.");
            }
        }
    }
}
