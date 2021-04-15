using System;
using System.IO;

namespace BackupAssistant.Core
{
    public partial class BackupAgent
    {
        private IBackupStarter _caller = null;
        private bool _cancelOperation = false;

        public BackupAgent(IBackupStarter caller)
        {
            _caller = caller ?? throw new ArgumentNullException(nameof(caller));
        }

        public void Cancel()
        {
            _cancelOperation = true;
        }

        private void ValidateBackup()
        {
            if (string.IsNullOrEmpty(_caller.SourcePath))
            {
                throw new ArgumentException("You must specify a backup source");
            }

            if (!Directory.Exists(_caller.SourcePath))
            {
                throw new ArgumentException("The specified source directory could not be found.");
            }

            if (string.IsNullOrEmpty(_caller.DestinationPath))
            {
                throw new ArgumentException("You must specify a backup destination");
            }

            if (!Directory.Exists(_caller.DestinationPath))
            {
                throw new ArgumentException("The specified destination directory could not be found.");
            }
        }
    }
}
