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
            if (!this.IsInSource && !this.IsInDestination)
            {
                return BackupAction.None;
            }
            else if (this.IsInSource && !this.IsInDestination)
            {
                return BackupAction.Copy;
            }
            else if (!this.IsInSource && this.IsInDestination)
            {
                return BackupAction.Delete;
            }
            else if (this.IsInSource && this.IsInDestination && this.SourceLastModified > this.DestinationLastModified)
            {
                return BackupAction.Overwrite;
            }
            else if (this.IsInSource && this.IsInDestination && this.SourceLastModified <= this.DestinationLastModified)
            {
                return BackupAction.None;
            }
            else
            {
                return BackupAction.None;
            }
        }
    }
}
