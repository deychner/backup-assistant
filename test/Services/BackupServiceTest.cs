using BackupAssistant.Test.Services.Base;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions;

namespace BackupAssistant.Test.Services
{
    public class BackupServiceTest : BackupServiceTestBase
    {
        public BackupServiceTest() : base(false) { }

        [Fact]
        public void SafeGetFileInfo_ThrowsException_LogsWarning()
        {
            string testFile = "test_file.txt";

            Mock<IFileInfoFactory> fileInfoFactoryMock = new();
            fileInfoFactoryMock
                .Setup(f => f.New(testFile))
                .Throws(new UnauthorizedAccessException("Access denied."));

            this.FileSystemMock?
                .Setup(fs => fs.FileInfo)
                .Returns(fileInfoFactoryMock.Object);

            SetupLogger("Failed to get file information");

            var fileInfo = this.BackupServiceInstance.SafeGetFileInfo(testFile);
            Assert.Null(fileInfo);
        }

        [Fact]
        public void SafeEnumerateFiles_ThrowsException_LogsWarning()
        {
            string testDirectory = @"c:\Source";

            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateFiles(testDirectory))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger("Failed to get files in directory");

            var files = this.BackupServiceInstance.SafeEnumerateFiles(testDirectory);
            Assert.Empty(files);
        }

        [Fact]
        public void SafeEnumerateDirectories_ThrowsException_LogsWarning()
        {
            string testDirectory = @"c:\Source";

            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateDirectories(testDirectory))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger("Failed to get directories in directory");

            var directories = this.BackupServiceInstance.SafeEnumerateDirectories(testDirectory);
            Assert.Empty(directories);
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
