using BackupAssistant.Services;
using BackupAssistant.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels.Base
{
    public class MainWindowViewModelTestBase : IDisposable
    {
        protected Mock<IBackupService> BackupServiceMock;
        protected Mock<ISettingsService> SettingsServiceMock;
        protected Mock<IDialogService> DialogServiceMock;
        protected Mock<ILogger<MainWindowViewModel>> LoggerMock;
        protected MockFileSystem InMemoryFileSystem;

        protected MainWindowViewModel? ViewModelInstance;

        public MainWindowViewModelTestBase(bool createInstance = true)
        {
            BackupServiceMock = new Mock<IBackupService>(MockBehavior.Strict);
            SettingsServiceMock = new Mock<ISettingsService>(MockBehavior.Strict);
            DialogServiceMock = new Mock<IDialogService>(MockBehavior.Strict);
            LoggerMock = new Mock<ILogger<MainWindowViewModel>>(MockBehavior.Strict);
            InMemoryFileSystem = new MockFileSystem();

            if (createInstance)
            {
                _ = SettingsServiceMock.SetupProperty(f => f.Filters, []);
                _ = SettingsServiceMock.SetupProperty(s => s.Source, null);
                _ = SettingsServiceMock.SetupProperty(d => d.Destination, null);
                _ = SettingsServiceMock.SetupProperty(b => b.BackupType);
                SettingsServiceMock.Setup(s => s.Save()).Verifiable();

                ViewModelInstance = new MainWindowViewModel(
                    BackupServiceMock.Object,
                    SettingsServiceMock.Object,
                    DialogServiceMock.Object,
                    LoggerMock.Object,
                    InMemoryFileSystem);
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
                BackupServiceMock.VerifyAll();
                SettingsServiceMock.VerifyAll();
                DialogServiceMock.VerifyAll();
                LoggerMock.VerifyAll();
            }
        }

        #endregion
    }
}
