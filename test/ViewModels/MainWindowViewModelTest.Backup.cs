using BackupAssistant.DataModels;
using BackupAssistant.Services;
using BackupAssistant.Test.ViewModels.Base;
using BackupAssistant.ViewModels;
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
        public async Task RunBackupAsync_Full()
        {
            SetupDirectories();

            _ = this.BackupServiceMock
                .Setup(s => s.RunFullBackupAsync(
                    @"c:\source",
                    @"c:\destination",
                    It.IsAny<ICollection<string>>(),
                    It.IsAny<IProgress<BackupProgress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            this.ViewModelInstance!.BackupType = BackupType.Full;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);
        }

        [Fact]
        public async Task RunBackupAsync_Incremental()
        {
            SetupDirectories();

            _ = this.BackupServiceMock
                .Setup(s => s.RunIncrementalBackupAsync(
                    @"c:\source",
                    @"c:\destination",
                    It.IsAny<ICollection<string>>(),
                    It.IsAny<IProgress<BackupProgress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            this.ViewModelInstance!.BackupType = BackupType.Incremental;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);
        }

        [Fact]
        public async Task RunBackupAsync_UnknownTypeDoesNothing()
        {
            SetupDirectories();

            this.ViewModelInstance!.BackupType = (BackupType)99;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            // Neither backup mode should have been invoked; the strict mock would have thrown
            Assert.Equal(string.Empty, this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_ReportsCancellation()
        {
            SetupDirectories();

            _ = this.BackupServiceMock
                .Setup(s => s.RunIncrementalBackupAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ICollection<string>>(),
                    It.IsAny<IProgress<BackupProgress>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            this.ViewModelInstance!.BackupType = BackupType.Incremental;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("Backup was canceled.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupAsync_SurfacesProgress()
        {
            SetupDirectories();

            _ = this.BackupServiceMock
                .Setup(s => s.RunFullBackupAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ICollection<string>>(),
                    It.IsAny<IProgress<BackupProgress>>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string _, string _, ICollection<string> _, IProgress<BackupProgress> progress, CancellationToken _) =>
                {
                    progress.Report(new BackupProgress { Progress = 42, IsIndeterminate = true, Status = "Working..." });
                    return Task.CompletedTask;
                });

            this.ViewModelInstance!.BackupType = BackupType.Full;

            // Progress<T> captures SynchronizationContext.Current when the view model constructs it
            // and dispatches reports through it. Left alone that is asynchronous, which would make
            // this test a race, so pin a context that invokes callbacks inline.
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

            Assert.Equal(42, this.ViewModelInstance.Progress);
            Assert.True(this.ViewModelInstance.ProgressBarIsIndeterminate);
            Assert.Equal("Working...", this.ViewModelInstance.Status);
        }

        /// <summary>
        /// Runs posted callbacks immediately on the calling thread, so that
        /// <see cref="Progress{T}"/> reports land synchronously.
        /// </summary>
        private sealed class InlineSynchronizationContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object? state) => d(state);

            public override void Send(SendOrPostCallback d, object? state) => d(state);
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
        public void BackupType_PersistsToSettings()
        {
            // The view model starts on Full, so switching to Incremental is a real change
            this.ViewModelInstance!.BackupType = BackupType.Incremental;

            Assert.Equal(BackupType.Incremental, this.ViewModelInstance.Model.BackupType);
            Assert.Equal((int)BackupType.Incremental, this.SettingsServiceMock.Object.BackupType);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void BackupType_SettingTheSameValueRaisesNoChange()
        {
            // A compiled two-way x:Bind writes the value straight back to the view model, so an
            // unchanged assignment must not raise PropertyChanged or the binding loops forever.
            int notifications = 0;
            this.ViewModelInstance!.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.BackupType)) notifications++;
            };

            this.ViewModelInstance.BackupType = this.ViewModelInstance.BackupType;

            Assert.Equal(0, notifications);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }

        [Fact]
        public void BackupTypes_ListsEveryMode()
        {
            Assert.Equal([BackupType.Full, BackupType.Incremental], this.ViewModelInstance!.BackupTypes);
        }

        [Theory]
        [InlineData(-5, 0)]
        [InlineData(0, 0)]
        [InlineData(50, 50)]
        [InlineData(100, 100)]
        [InlineData(150, 100)]
        public void Progress_IsClampedToRange(int value, int expected)
        {
            this.ViewModelInstance!.Progress = value;

            Assert.Equal(expected, this.ViewModelInstance.Progress);
        }

        [Fact]
        public void CancelRunBackupCommand_IsStable()
        {
            // The view binds to this once, so it must not hand back a new command each read
            Assert.Same(this.ViewModelInstance!.CancelRunBackupCommand, this.ViewModelInstance.CancelRunBackupCommand);
        }

        private void SetupDirectories()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source");
            this.InMemoryFileSystem.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = @"c:\destination";
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
