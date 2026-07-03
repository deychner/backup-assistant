using BackupAssistant.Test.Services.Base;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions;

namespace BackupAssistant.Test.Services
{
    public class BackupServiceTest : BackupServiceTestBase
    {
        private const string TEST_FILE = @"c:\test_file.txt";
        private const string TEST_DIRECTORY = @"c:\Source";

        public BackupServiceTest() : base(false) { }

        [Fact]
        public void SafeGetFileInfo_ThrowsException_LogsWarning()
        {
            Mock<IFileInfoFactory> fileInfoFactoryMock = new();
            fileInfoFactoryMock
                .Setup(f => f.New(TEST_FILE))
                .Throws(new UnauthorizedAccessException("Access denied."));

            this.FileSystemMock?
                .Setup(fs => fs.FileInfo)
                .Returns(fileInfoFactoryMock.Object);

            SetupLogger("Failed to get file information");

            var fileInfo = this.BackupServiceInstance.SafeGetFileInfo(TEST_FILE);
            Assert.Null(fileInfo);
        }

        [Fact]
        public void SafeEnumerateFiles_ThrowsException_LogsWarning()
        {
            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateFiles(TEST_DIRECTORY))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger("Failed to get files in directory");

            var files = this.BackupServiceInstance.SafeEnumerateFiles(TEST_DIRECTORY);
            Assert.Empty(files);
        }

        [Fact]
        public void SafeEnumerateDirectories_ThrowsException_LogsWarning()
        {
            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateDirectories(TEST_DIRECTORY))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger("Failed to get directories in directory");

            var directories = this.BackupServiceInstance.SafeEnumerateDirectories(TEST_DIRECTORY);
            Assert.Empty(directories);
        }

        [Fact]
        public async Task SafeCopyFileAsync_ThrowsException_LogsWarning()
        {
            this.FileSystemMock?
                .Setup(x => x.Path.GetDirectoryName(TEST_FILE))
                .Returns(TEST_DIRECTORY);

            this.FileSystemMock?
                .Setup(x => x.Directory.Exists(TEST_DIRECTORY))
                .Returns(true);

            this.FileSystemMock?
                .Setup(x => x.File.Copy(It.IsAny<string>(), TEST_FILE, It.IsAny<bool>()))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger("Copy file failed for file");

            await this.BackupServiceInstance.SafeCopyFileAsync(TEST_FILE, TEST_FILE, true);
        }

        [Fact]
        public async Task SafeDeleteFileAsync_ThrowsException_LogsWarning()
        {
            this.FileSystemMock?
                .Setup(x => x.File.Delete(TEST_FILE))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger("Delete file failed for file");

            await this.BackupServiceInstance.SafeDeleteFileAsync(TEST_FILE);
        }

        private void SetupLogger(string messageSearchText)
        {
            LoggerMock.Setup(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, type) => value.ToString()!.Contains(messageSearchText)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }
    }
}
