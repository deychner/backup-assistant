using BackupAssistant.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.Services.Base
{
    public class BackupServiceTestBase : IDisposable
    {
        protected MockFileSystem? InMemoryFileSystem;
        protected Mock<IFileSystem>? FileSystemMock;
        protected Mock<ILogger<BackupService>> LoggerMock;
        protected BackupService BackupServiceInstance;

        public BackupServiceTestBase(bool useInMemoryFileSystem = true)
        {
            LoggerMock = new Mock<ILogger<BackupService>>(MockBehavior.Strict);

            if (useInMemoryFileSystem)
            {
                InMemoryFileSystem = new MockFileSystem();
                BackupServiceInstance = new BackupService(InMemoryFileSystem, LoggerMock.Object);
            }
            else
            {
                FileSystemMock = new Mock<IFileSystem>(MockBehavior.Strict);
                BackupServiceInstance = new BackupService(FileSystemMock.Object, LoggerMock.Object);
            }
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
                FileSystemMock?.VerifyAll();
                LoggerMock.VerifyAll();
            }
        }

        #endregion
    }
}