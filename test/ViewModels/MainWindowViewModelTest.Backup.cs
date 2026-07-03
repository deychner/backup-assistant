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
            SetupLogger();

            this.FileSystemMock.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = string.Empty;
            this.ViewModelInstance.Destination = @"c:\destination";

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The source directory does not exist.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_DestinationDoesNotExist()
        {
            SetupLogger();

            this.FileSystemMock.AddDirectory(@"c:\source");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = string.Empty;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The destination directory does not exist.", this.ViewModelInstance.Status);
        }

        private void SetupLogger()
        {
            this.LoggerMock.Setup(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }
    }
}
