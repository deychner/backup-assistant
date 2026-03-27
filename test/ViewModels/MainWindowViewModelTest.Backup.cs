using BackupAssistant.Test.ViewModels.Base;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackup : MainWindowViewModelTestBase
    {
        [Fact]
        public async Task RunBackupAsync_SourceDoesNotExist()
        {
            this.LogServiceMock.Setup(c => c.ClearLog());
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));
            this.LogServiceMock.Setup(w => w.WriteLogEntry()).Verifiable();

            this.FileSystemMock.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = string.Empty;
            this.ViewModelInstance.Destination = @"c:\destination";

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The source directory does not exist.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_DestinationDoesNotExist()
        {
            this.LogServiceMock.Setup(c => c.ClearLog());
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));
            this.LogServiceMock.Setup(w => w.WriteLogEntry()).Verifiable();

            this.FileSystemMock.AddDirectory(@"c:\source");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = string.Empty;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The destination directory does not exist.", this.ViewModelInstance.Status);
        }
    }
}
