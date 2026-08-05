using BackupAssistant.DataModels;
using BackupAssistant.Services;
using BackupAssistant.Test.TestHelpers;
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

        [Fact]
        public async Task RunBackupAsync_Full_RunsFullBackup()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
            this.ViewModelInstance.BackupType = BackupType.Full;

            _ = this.BackupServiceMock
                .Setup(b => b.RunFullBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            this.BackupServiceMock.Verify(
                b => b.RunFullBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RunBackupAsync_Incremental_RunsIncrementalBackup()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
            this.ViewModelInstance.BackupType = BackupType.Incremental;

            _ = this.BackupServiceMock
                .Setup(b => b.RunIncrementalBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            this.BackupServiceMock.Verify(
                b => b.RunIncrementalBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RunBackupAsync_UnknownBackupType_DoesNothing()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
            this.ViewModelInstance.BackupType = (BackupType)99;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            this.BackupServiceMock.Verify(
                b => b.RunFullBackupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()),
                Times.Never);
            this.BackupServiceMock.Verify(
                b => b.RunIncrementalBackupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task RunBackupAsync_OperationCanceled_SetsCanceledStatus()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
            this.ViewModelInstance.BackupType = BackupType.Full;

            _ = this.BackupServiceMock
                .Setup(b => b.RunFullBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("Backup was canceled.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_ProgressReachesOneHundred_DespiteOutOfOrderReports()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
            this.ViewModelInstance.BackupType = BackupType.Full;

            _ = this.BackupServiceMock
                .Setup(b => b.RunFullBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, ICollection<string>, IProgress<BackupProgress>, CancellationToken>((s, d, f, progress, token) =>
                {
                    // Simulate concurrent workers whose reports are delivered out of order,
                    // followed by the guaranteed final report the A1 fix adds after Task.WhenAll.
                    progress.Report(new BackupProgress { Progress = 60 });
                    progress.Report(new BackupProgress { Progress = 80 });
                    progress.Report(new BackupProgress { Progress = 40 });
                    progress.Report(new BackupProgress { Progress = 100, Status = "Backup is complete." });
                    return Task.CompletedTask;
                });

            SynchronizationContext? original = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new InlineSynchronizationContext());
            try
            {
                await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(original);
            }

            Assert.Equal(100, this.ViewModelInstance.Progress);
            Assert.Equal("Backup is complete.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_SurfacesIndeterminateAndStatus()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
            this.ViewModelInstance.BackupType = BackupType.Full;

            _ = this.BackupServiceMock
                .Setup(b => b.RunFullBackupAsync(@"c:\source", @"c:\destination", It.IsAny<ICollection<string>>(), It.IsAny<IProgress<BackupProgress>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, ICollection<string>, IProgress<BackupProgress>, CancellationToken>((s, d, f, progress, token) =>
                {
                    progress.Report(new BackupProgress { IsIndeterminate = true, Status = "Getting source file list..." });
                    return Task.CompletedTask;
                });

            SynchronizationContext? original = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new InlineSynchronizationContext());
            try
            {
                await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(original);
            }

            Assert.True(this.ViewModelInstance.ProgressBarIsIndeterminate);
            Assert.Equal("Getting source file list...", this.ViewModelInstance.Status);
        }

        [Fact]
        public void CanRunBackup()
        {
            Assert.False(this.ViewModelInstance!.CanRunBackup());

            this.ViewModelInstance.Source = @"c:\source";
            Assert.False(this.ViewModelInstance.CanRunBackup());

            this.ViewModelInstance.Destination = @"c:\destination";
            Assert.True(this.ViewModelInstance.CanRunBackup());
        }

        [Fact]
        public void BackupTypes_ListsEveryMode()
        {
            Assert.Equal([BackupType.Full, BackupType.Incremental], this.ViewModelInstance!.BackupTypes);
        }

        [Fact]
        public void BackupType_Setter_SavesSettings_WhenChanged()
        {
            this.ViewModelInstance!.BackupType = BackupType.Incremental;

            this.SettingsServiceMock.VerifySet(s => s.BackupType = (int)BackupType.Incremental, Times.Once);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void BackupType_Setter_DoesNotSaveSettings_WhenUnchanged()
        {
            this.ViewModelInstance!.Model.BackupType = BackupType.Full;

            this.ViewModelInstance.BackupType = BackupType.Full;

            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }

        [Fact]
        public void CancelRunBackupCommand_ReturnsSameInstanceOnRepeatedReads()
        {
            Assert.Same(this.ViewModelInstance!.CancelRunBackupCommand, this.ViewModelInstance.CancelRunBackupCommand);
        }

        [Theory]
        [InlineData(-50, 0)]
        [InlineData(0, 0)]
        [InlineData(50, 50)]
        [InlineData(100, 100)]
        [InlineData(150, 100)]
        public void Progress_ClampsToValidRange(int input, int expected)
        {
            this.ViewModelInstance!.Progress = input;

            Assert.Equal(expected, this.ViewModelInstance.Progress);
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
