using System;
using System.IO.Abstractions;

namespace BackupAssistant.Core
{
    public partial class BackupAgent
    {
        private IBackupStarter _caller = null;
        private IFileSystem _fileSystem = null;

        private bool _cancelOperation = false;

        public BackupAgent(IBackupStarter caller) : this(
            caller: caller,
            fileSystem: new FileSystem() // Use System.IO implementation
            )
        {

        }

        public BackupAgent(IBackupStarter caller, IFileSystem fileSystem)
        {
            _caller = caller ?? throw new ArgumentNullException(nameof(caller));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public void Cancel()
        {
            _cancelOperation = true;
        }

        public void ValidateBackup()
        {
            if (string.IsNullOrEmpty(_caller.SourcePath))
            {
                throw new ArgumentException("You must specify a backup source.");
            }

            if (!_fileSystem.Directory.Exists(_caller.SourcePath))
            {
                throw new ArgumentException("The specified source directory could not be found.");
            }

            if (string.IsNullOrEmpty(_caller.DestinationPath))
            {
                throw new ArgumentException("You must specify a backup destination.");
            }

            if (!_fileSystem.Directory.Exists(_caller.DestinationPath))
            {
                throw new ArgumentException("The specified destination directory could not be found.");
            }
        }
    }
}
