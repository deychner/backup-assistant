using BackupAssistant.Test.ViewModels.Base;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestMenu : MainWindowViewModelTestBase
    {
        [Fact]
        public void ExitCommand_ShutsDownApplication()
        {
            _ = this.ApplicationServiceMock.Setup(a => a.Shutdown());

            this.ViewModelInstance!.ExitCommand.Execute(null);

            this.ApplicationServiceMock.Verify(a => a.Shutdown(), Times.Once);
        }

        [Fact]
        public void ExitCommand_ReturnsSameInstanceOnRepeatedReads()
        {
            Assert.Same(this.ViewModelInstance!.ExitCommand, this.ViewModelInstance.ExitCommand);
        }

        [Fact]
        public void AboutCommand_ShowsAboutDialog()
        {
            _ = this.DialogServiceMock.Setup(d => d.ShowDialog<About>(It.IsAny<object>())).Returns(true);

            this.ViewModelInstance!.AboutCommand.Execute(null);

            this.DialogServiceMock.Verify(d => d.ShowDialog<About>(It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public void AboutCommand_ReturnsSameInstanceOnRepeatedReads()
        {
            Assert.Same(this.ViewModelInstance!.AboutCommand, this.ViewModelInstance.AboutCommand);
        }
    }
}
