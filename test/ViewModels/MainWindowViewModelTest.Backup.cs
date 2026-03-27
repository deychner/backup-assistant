using BackupAssistant.Test.ViewModels.Base;
using Moq;
using System.IO.Abstractions;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackup : MainWindowViewModelTestBase
    {
        [Fact]
        public async Task RunBackupInternal_SourceDoesNotExist()
        {
            _ = this.LogServiceMock.Setup(c => c.ClearLog());
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            this.FileSystemMock.AddDirectory(@"c:\destination");

            this.ViewModelInstance!.Source = string.Empty;
            this.ViewModelInstance.Destination = @"c:\destination";

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The source directory does not exist.", this.ViewModelInstance.Status);
        }

        [Fact]
        public async Task RunBackupInternal_DestinationDoesNotExist()
        {
            _ = this.LogServiceMock.Setup(c => c.ClearLog());
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            this.FileSystemMock.AddDirectory(@"c:\source");

            this.ViewModelInstance!.Source = @"c:\source";
            this.ViewModelInstance.Destination = string.Empty;

            await this.ViewModelInstance.RunBackupAsync(CancellationToken.None);

            Assert.Equal("The destination directory does not exist.", this.ViewModelInstance.Status);
        }

        [Fact]
        public void SafeGetFileInfo()
        {
            IFileInfo? fileInfo = null;
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                fileInfo = this.ViewModelInstance!.SafeGetFileInfo(string.Empty);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.Null(fileInfo);
        }

        [Fact]
        public void SafeEnumerateFiles()
        {
            ICollection<string>? files = null;
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                files = [.. this.ViewModelInstance!.SafeEnumerateFiles(Guid.NewGuid().ToString())];
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.NotNull(files);
            Assert.Empty(files);
        }

        [Fact]
        public void SafeEnumerateDirectories()
        {
            ICollection<string>? directories = null;
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                directories = [.. this.ViewModelInstance!.SafeEnumerateDirectories(Guid.NewGuid().ToString())];
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.NotNull(directories);
            Assert.Empty(directories);
        }

        [Fact]
        public void EnsureDirectoryPathExists()
        {
            this.FileSystemMock.AddDirectory(@"c:\");

            try
            {
                this.ViewModelInstance!.EnsureDirectoryPathExists(@"c:\level1\level2\file.txt");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.True(this.FileSystemMock.Directory.Exists(@"c:\level1"), "Top level directory was not created.");
            Assert.True(this.FileSystemMock.Directory.Exists(@"c:\level1\level2"), "Top level directory was not created.");
        }

        [Fact]
        public async Task SafeCopyFile()
        {
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                await this.ViewModelInstance!.SafeCopyFileAsync(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }

        [Fact]
        public async Task SafeDeleteFile()
        {
            _ = this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                await this.ViewModelInstance!.SafeDeleteFileAsync(string.Empty);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }
    }
}
