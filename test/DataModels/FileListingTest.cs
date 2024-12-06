using BackupAssistant.DataModels;

namespace BackupAssistant.Test.DataModels
{

    public class FileListingTest
    {
        [Fact]
        public void GetBackupAction_None()
        {
            FileListing fileListing = new()
            {
                IsInSource = false,
                IsInDestination = false,
                SourceLastModified = DateTime.Today,
                DestinationLastModified = DateTime.Today
            };

            Assert.Equal(BackupAction.None, fileListing.GetBackupAction());

            FileListing fileListing2 = new()
            {
                IsInSource = true,
                IsInDestination = true,
                SourceLastModified = DateTime.Today.AddDays(-1),
                DestinationLastModified = DateTime.Today
            };

            Assert.Equal(BackupAction.None, fileListing2.GetBackupAction());
        }

        [Fact]
        public void GetBackupAction_Copy()
        {
            FileListing fileListing = new()
            {
                IsInSource = true,
                IsInDestination = false,
                SourceLastModified = DateTime.Today,
                DestinationLastModified = DateTime.Today
            };

            Assert.Equal(BackupAction.Copy, fileListing.GetBackupAction());
        }

        [Fact]
        public void GetBackupAction_Delete()
        {
            FileListing fileListing = new()
            {
                IsInSource = false,
                IsInDestination = true,
                SourceLastModified = DateTime.Today,
                DestinationLastModified = DateTime.Today
            };

            Assert.Equal(BackupAction.Delete, fileListing.GetBackupAction());
        }

        [Fact]
        public void GetBackupAction_Overwrite()
        {
            FileListing fileListing = new()
            {
                IsInSource = true,
                IsInDestination = true,
                SourceLastModified = DateTime.Today,
                DestinationLastModified = DateTime.Today.AddDays(-1)
            };

            Assert.Equal(BackupAction.Overwrite, fileListing.GetBackupAction());
        }
    }
}
