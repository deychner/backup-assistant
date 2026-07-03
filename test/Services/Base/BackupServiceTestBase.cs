using BackupAssistant.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.Services.Base
{
    public class BackupServiceTestBase : IDisposable
    {
        protected Mock<ILogger<BackupService>> LoggerMock;
        protected MockFileSystem FileSystemMock;
        protected BackupService BackupServiceInstance;

        public BackupServiceTestBase()
        {
            LoggerMock = new Mock<ILogger<BackupService>>(MockBehavior.Strict);
            FileSystemMock = new MockFileSystem();

            BackupServiceInstance = new BackupService(FileSystemMock, LoggerMock.Object);
        }

        #region IDisposable support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                LoggerMock.VerifyAll();
            }
        }

        #endregion
    }
}