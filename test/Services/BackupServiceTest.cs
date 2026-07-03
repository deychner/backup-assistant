using BackupAssistant.Test.Services.Base;
using Microsoft.Extensions.Logging;
using Moq;

namespace BackupAssistant.Test.Services
{
    public class BackupServiceTest : BackupServiceTestBase
    {
        #region SafeEnumerateDirectories

        [Fact]
        public async Task SafeEnumerateDirectories_ThrowsException_LogsWarning()
        {
            var testDirectory = @"c:\Source";

            this.FileSystemMock?
                .Setup(x => x.Directory.EnumerateDirectories(testDirectory))
                .Throws(new UnauthorizedAccessException("Access denied"));

            SetupLogger();

            var fileList = await this.BackupServiceInstance.GetFileListAsync(testDirectory, [], CancellationToken.None);

            Assert.Empty(fileList);
        }

        #endregion

        private void SetupLogger()
        {
            LoggerMock.Setup(x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
        }
    }
}
