using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels.Base
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
            SettingsServiceMock = new Mock<ISettingsService>(MockBehavior.Strict);
            DialogServiceMock = new Mock<IDialogService>(MockBehavior.Strict);
            LogServiceMock = new Mock<ILogService>(MockBehavior.Strict);
            FileSystemMock = new MockFileSystem();

            if (createInstance)
            {
                _ = SettingsServiceMock.SetupProperty(f => f.Filters, []);
                _ = SettingsServiceMock.SetupProperty(s => s.Source, null);
                _ = SettingsServiceMock.SetupProperty(d => d.Destination, null);
                _ = SettingsServiceMock.SetupProperty(b => b.BackupType);
                SettingsServiceMock.Setup(s => s.Save()).Verifiable();

                ViewModelInstance = new MainWindowViewModel(SettingsServiceMock.Object, DialogServiceMock.Object, LogServiceMock.Object, FileSystemMock);
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
