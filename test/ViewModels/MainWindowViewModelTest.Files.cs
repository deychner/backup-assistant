using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestFiles : MainWindowViewModelTestBase
    {
        [Fact]
        public void AddEditSource()
        {
            this.ViewModelInstance!.Model.Source = "old source";
            this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(It.IsAny<string>())).Returns(() => (true, "new source"));

            this.ViewModelInstance.AddEditSource();

            Assert.Equal("new source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public void AddEditSource_NoAction()
        {
            this.ViewModelInstance!.Model.Source = "old source";
            this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(It.IsAny<string>())).Returns(() => (false, "new source"));

            this.ViewModelInstance.AddEditSource();

            Assert.Equal("old source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public void AddEditDestination()
        {
            this.ViewModelInstance!.Model.Destination = "old destination";
            this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(It.IsAny<string>())).Returns(() => (true, "new destination"));

            this.ViewModelInstance.AddEditDestination();

            Assert.Equal("new destination", this.ViewModelInstance.Model.Destination);
        }

        [Fact]
        public void AddEditDestination_NoAction()
        {
            this.ViewModelInstance!.Model.Destination = "old destination";
            this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(It.IsAny<string>())).Returns(() => (false, "new destination"));

            this.ViewModelInstance.AddEditDestination();

            Assert.Equal("old destination", this.ViewModelInstance.Model.Destination);
        }
    }
}
