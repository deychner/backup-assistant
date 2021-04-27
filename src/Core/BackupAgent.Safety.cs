using System.Collections.Generic;
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

        public IReadOnlyCollection<string> SafeGetFiles(string directory)
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

        public IReadOnlyCollection<string> SafeGetDirectories(string directory)
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

        public void EnsureDirectoryPathExists(string path)
        {
            string directory = _fileSystem.Path.GetDirectoryName(path);

            if (!_fileSystem.Directory.Exists(directory))
            {
                _fileSystem.Directory.CreateDirectory(directory);
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
