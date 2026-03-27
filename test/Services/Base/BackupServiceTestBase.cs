using BackupAssistant.Services;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.Services.Base
{
    public class BackupServiceTestBase : IDisposable
    {
        protected Mock<ILogService> LogServiceMock;
        protected MockFileSystem FileSystemMock;
        protected BackupService BackupServiceInstance;

        public BackupServiceTestBase()
        {
            LogServiceMock = new Mock<ILogService>(MockBehavior.Strict);
            FileSystemMock = new MockFileSystem();
            
            BackupServiceInstance = new BackupService(FileSystemMock, LogServiceMock.Object);
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
                LogServiceMock.VerifyAll();
            }
        }

        #endregion
    }
}