using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBase : IDisposable
    {
        protected Mock<ISettingsService> SettingsServiceMock;
        protected Mock<IDialogService> DialogServiceMock;
        protected Mock<ILogService> LogServiceMock;
        protected MockFileSystem FileSystemMock;

        protected MainWindowViewModel? ViewModelInstance;

        public MainWindowViewModelTestBase(bool createInstance = true)
        {
            this.SettingsServiceMock = new Mock<ISettingsService>(MockBehavior.Strict);
            this.DialogServiceMock = new Mock<IDialogService>(MockBehavior.Strict);
            this.LogServiceMock = new Mock<ILogService>(MockBehavior.Strict);
            this.FileSystemMock = new MockFileSystem();

            if (createInstance)
            {
                this.ViewModelInstance = new MainWindowViewModel(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);
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
                SettingsServiceMock.VerifyAll();
                DialogServiceMock.VerifyAll();
                LogServiceMock.VerifyAll();
            }
        }

        #endregion
    }
}
