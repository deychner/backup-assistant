using BackupAssistant.Services;
using BackupAssistant.Test.Services.Base;
using BackupAssistant.Test.TestHelpers;
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
        public async Task RunFullBackupAsync_AbortsWhenSourceEnumerationFails()
        {
            const string source = @"c:\Source";
            const string destination = @"c:\Destination";

            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateFiles(source))
                .Throws(new UnauthorizedAccessException("Access denied."));

            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateDirectories(source))
                .Throws(new UnauthorizedAccessException("Access denied."));

            SetupLogger(LogLevel.Warning, "Failed to get files in directory");
            SetupLogger(LogLevel.Warning, "Failed to get directories in directory");
            SetupLogger(LogLevel.Error, "could not be fully read");

            List<BackupProgress> reports = [];
            IProgress<BackupProgress> progress = new Progress<BackupProgress>(reports.Add);

            SynchronizationContext? original = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new InlineSynchronizationContext());
            try
            {
                // Note: Directory.Exists/Delete for the destination are intentionally not set up
                // on this strict mock. If the abort guard failed to short-circuit before the
                // delete step, this call would throw a MockException instead of returning.
                await this.BackupServiceInstance.RunFullBackupAsync(source, destination, [], progress, CancellationToken.None);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(original);
            }

            Assert.Contains(reports, r => r.Status != null && r.Status.Contains("aborted"));
        }

        [Fact]
        public async Task SafeCopyFileAsync_CreatesDestinationDirectory_WhenMissing()
        {
            const string destinationFile = @"c:\Destination\sub\file.txt";
            const string destinationDirectory = @"c:\Destination\sub";

            this.FileSystemMock?.Setup(x => x.Path.GetDirectoryName(destinationFile)).Returns(destinationDirectory);
            this.FileSystemMock?.Setup(x => x.Directory.Exists(destinationDirectory)).Returns(false);
            this.FileSystemMock?.Setup(x => x.Directory.CreateDirectory(destinationDirectory)).Returns((IDirectoryInfo)null!);
            this.FileSystemMock?.Setup(x => x.File.Copy(TEST_FILE, destinationFile, false));

            await this.BackupServiceInstance.SafeCopyFileAsync(TEST_FILE, destinationFile, false);

            this.FileSystemMock!.Verify(x => x.Directory.CreateDirectory(destinationDirectory), Times.Once);
        }

        [Fact]
        public async Task SafeCopyFileAsync_SkipsCreatingDirectory_WhenAlreadyExists()
        {
            const string destinationFile = @"c:\Destination\file.txt";
            const string destinationDirectory = @"c:\Destination";

            this.FileSystemMock?.Setup(x => x.Path.GetDirectoryName(destinationFile)).Returns(destinationDirectory);
            this.FileSystemMock?.Setup(x => x.Directory.Exists(destinationDirectory)).Returns(true);
            this.FileSystemMock?.Setup(x => x.File.Copy(TEST_FILE, destinationFile, false));

            await this.BackupServiceInstance.SafeCopyFileAsync(TEST_FILE, destinationFile, false);

            this.FileSystemMock!.Verify(x => x.Directory.CreateDirectory(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SafeCopyFileAsync_SkipsCreatingDirectory_WhenDestinationHasNoDirectoryPart()
        {
            const string destinationFile = "file.txt";

            this.FileSystemMock?.Setup(x => x.Path.GetDirectoryName(destinationFile)).Returns((string?)null);
            this.FileSystemMock?.Setup(x => x.File.Copy(TEST_FILE, destinationFile, false));

            await this.BackupServiceInstance.SafeCopyFileAsync(TEST_FILE, destinationFile, false);

            this.FileSystemMock!.Verify(x => x.Directory.Exists(It.IsAny<string>()), Times.Never);
        }

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
            SetupLogger(LogLevel.Warning, messageSearchText);
        }

        private void SetupLogger(LogLevel level, string messageSearchText)
        {
            LoggerMock.Setup(x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, type) => value.ToString()!.Contains(messageSearchText)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }
    }
}
