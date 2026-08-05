using System;

namespace BackupAssistant.DataModels
{
    public enum BackupAction : byte
    {
        None,
        Copy,
        Delete,
        Overwrite
    }

    public class FileListing
    {
        public bool IsInSource { get; set; }
        public bool IsInDestination { get; set; }
        public DateTime SourceLastModified { get; set; }
        public DateTime DestinationLastModified { get; set; }
        public long Size { get; set; }

        public FileListing()
        {
            this.IsInSource = false;
            this.IsInDestination = false;
            this.SourceLastModified = DateTime.MinValue;
            this.DestinationLastModified = DateTime.MinValue;
            this.Size = 0L;
        }

        public BackupAction GetBackupAction()
        {
            return (this.IsInSource, this.IsInDestination) switch
            {
                (false, false) => BackupAction.None,
                (true, false) => BackupAction.Copy,
                (false, true) => BackupAction.Delete,
                (true, true) when this.SourceLastModified > this.DestinationLastModified => BackupAction.Overwrite,
                (true, true) => BackupAction.None,
            };
        }
    }
}
