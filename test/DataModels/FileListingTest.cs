using BackupAssistant.DataModels;

namespace BackupAssistant.Test.DataModels
{

    public class FileListingTest
    {
        [Fact]
        public void NotSource_NotDestination()
        {
            FileListing fileListing = new()
            {
                IsInSource = false,
                IsInDestination = false,
                SourceLastModified = DateTime.Today,
                DestinationLastModified = DateTime.Today
            };

            Assert.Equal(BackupAction.None, fileListing.GetBackupAction());
        }

        [Fact]
        public void Source_NotDestination()
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
        public void NotSource_Destination()
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
        public void Source_Destination_SourceNewer()
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

        [Fact]
        public void Source_Destination_DestinationNewer()
        {
            FileListing fileListing = new()
            {
                IsInSource = true,
                IsInDestination = true,
                SourceLastModified = DateTime.Today.AddDays(-1),
                DestinationLastModified = DateTime.Today
            };

            Assert.Equal(BackupAction.None, fileListing.GetBackupAction());
        }
    }
}
