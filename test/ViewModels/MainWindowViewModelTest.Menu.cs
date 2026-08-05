using BackupAssistant.Test.ViewModels.Base;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestMenu : MainWindowViewModelTestBase
    {
        [Fact]
        public void ExitCommand()
        {
            this.ApplicationServiceMock.Setup(a => a.Exit());

            this.ViewModelInstance!.ExitCommand.Execute(null);

            this.ApplicationServiceMock.Verify(a => a.Exit(), Times.Once);
        }

        [Fact]
        public async Task AboutCommand()
        {
            _ = this.DialogServiceMock.Setup(d => d.ShowAboutDialogAsync()).Returns(Task.CompletedTask);

            await this.ViewModelInstance!.AboutCommand.ExecuteAsync(null);

            this.DialogServiceMock.Verify(d => d.ShowAboutDialogAsync(), Times.Once);
        }
    }
}
