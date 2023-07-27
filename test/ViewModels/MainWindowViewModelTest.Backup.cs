using Moq;
using System.IO.Abstractions;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackup : MainWindowViewModelTestBase
    {
        [Fact]
        public void SafeGetFileInfo()
        {
            IFileInfo? fileInfo = null;
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

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
        public void SafeGetFiles()
        {
            IReadOnlyCollection<string>? files = null;
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                files = this.ViewModelInstance!.SafeGetFiles(Guid.NewGuid().ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.NotNull(files);
            Assert.Empty(files);
        }

        [Fact]
        public void SafeGetDirectories()
        {
            IReadOnlyCollection<string>? directories = null;
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                directories = this.ViewModelInstance!.SafeGetDirectories(Guid.NewGuid().ToString());
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
        public void SafeCopyFile()
        {
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                this.ViewModelInstance!.SafeCopyFile(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }

        [Fact]
        public void SafeDeleteFile()
        {
            this.LogServiceMock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            try
            {
                this.ViewModelInstance!.SafeDeleteFile(string.Empty);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }
    }
}
