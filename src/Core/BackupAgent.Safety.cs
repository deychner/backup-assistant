using System.Collections.ObjectModel;
using System.IO;

namespace BackupAssistant.Core
{
    public partial class BackupAgent
    {
        public FileInfo SafeGetFileInfo(string file)
        {
            try
            {
                return new FileInfo(file);
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
                var files = Directory.GetFiles(directory);
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
                var directories = Directory.GetDirectories(directory);
                return new ReadOnlyCollection<string>(directories);
            }
            catch
            {
                _caller.AddToLogEntry($"Failed to get directories in directory '{directory}'.");

                return new ReadOnlyCollection<string>(new string[] { });
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
                File.Copy(sourceFileName, destinationFileName, overwrite);
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
                File.Delete(file);
            }
            catch
            {
                _caller.AddToLogEntry($"Delete file failed for file '{file}'.");
            }
        }
    }
}
