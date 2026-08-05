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
        protected Mock<IApplicationService> ApplicationServiceMock;
        protected Mock<ILogger<MainWindowViewModel>> LoggerMock;
        protected MockFileSystem InMemoryFileSystem;

        protected MainWindowViewModel? ViewModelInstance;

        public MainWindowViewModelTestBase(bool createInstance = true)
        {
            BackupServiceMock = new Mock<IBackupService>(MockBehavior.Strict);
            SettingsServiceMock = new Mock<ISettingsService>(MockBehavior.Strict);
            DialogServiceMock = new Mock<IDialogService>(MockBehavior.Strict);
            ApplicationServiceMock = new Mock<IApplicationService>(MockBehavior.Strict);
            LoggerMock = new Mock<ILogger<MainWindowViewModel>>(MockBehavior.Strict);
            InMemoryFileSystem = new MockFileSystem();

            if (createInstance)
            {
                _ = SettingsServiceMock.SetupProperty(f => f.Filters, []);
                _ = SettingsServiceMock.SetupProperty(s => s.Source, null);
                _ = SettingsServiceMock.SetupProperty(d => d.Destination, null);
                _ = SettingsServiceMock.SetupProperty(b => b.BackupType);

                // Constructing the view model no longer writes settings back out, so Save() is
                // allowed rather than required here. Tests that care assert on it themselves.
                SettingsServiceMock.Setup(s => s.Save());

                ViewModelInstance = CreateViewModel();
            }
        }

        protected MainWindowViewModel CreateViewModel()
        {
            return new MainWindowViewModel(
                BackupServiceMock.Object,
                SettingsServiceMock.Object,
                DialogServiceMock.Object,
                ApplicationServiceMock.Object,
                LoggerMock.Object,
                InMemoryFileSystem);
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
                // Verify() checks the setups a test explicitly marked Verifiable. Unexpected calls
                // are already caught by MockBehavior.Strict, so there is no need for VerifyAll()
                // to additionally demand that every shared setup in this base class gets used.
                BackupServiceMock.Verify();
                SettingsServiceMock.Verify();
                DialogServiceMock.Verify();
                ApplicationServiceMock.Verify();
                LoggerMock.Verify();
            }
        }

        #endregion
    }
}
