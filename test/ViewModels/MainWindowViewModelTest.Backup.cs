using BackupAssistant.Test.ViewModels.Base;
using Microsoft.Extensions.Logging;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackup : MainWindowViewModelTestBase
    {
        [Fact]
        public async Task RunBackupAsync_SourceDoesNotExist()
        {
            SetupLogger("Backup failed. The source directory 'invalid_directory' does not exist.");

            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = "invalid_directory";
            this.ViewModelInstance.Destination = @"c:\destination";

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The source directory does not exist.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_DestinationDoesNotExist()
        {
            SetupLogger("Backup failed. The destination directory 'invalid_directory' does not exist.");

            this.InMemoryFileSystem.AddDirectory(@"c:\source");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = "invalid_directory";

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The destination directory does not exist.", this.ViewModelInstance.Status);
        }

        private void SetupLogger(string messageSearchText)
        {
            this.LoggerMock.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, type) => value.ToString()!.Contains(messageSearchText)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }
    }
}
