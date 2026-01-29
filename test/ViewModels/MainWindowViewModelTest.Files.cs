using BackupAssistant.Test.ViewModels.Base;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestFiles : MainWindowViewModelTestBase
    {
        [Fact]
        public void AddEditSource()
        {
            this.FileSystemMock.AddDirectory(@"c:\old\source");

            this.ViewModelInstance!.Model.Source = @"c:\old\source";
            // Use c:\old\source as the initial path to ensure that the default path for an existing directory is used
            _ = this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(@"c:\old\source")).Returns(() => (true, @"c:\new\source"));

            this.ViewModelInstance.AddEditSource();

            Assert.Equal(@"c:\new\source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public void AddEditSource_SourceDirectoryDoesNotExist()
        {
            this.ViewModelInstance!.Model.Source = @"c:\old\source";
            // Use C:\ as the initial path to ensure that the default path for a non-existent directory is used
            _ = this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(@"C:\")).Returns(() => (true, @"c:\new\source"));

            this.ViewModelInstance.AddEditSource();

            Assert.Equal(@"c:\new\source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public void AddEditSource_NoAction()
        {
            this.ViewModelInstance!.Model.Source = @"c:\old\source";
            // Use C:\ as the initial path to ensure that the default path for a non-existent directory is used
            _ = this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(@"C:\")).Returns(() => (false, @"c:\new\source"));

            this.ViewModelInstance.AddEditSource();

            Assert.Equal(@"c:\old\source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public void AddEditDestination()
        {
            this.FileSystemMock.AddDirectory(@"c:\old\destination");

            this.ViewModelInstance!.Model.Destination = @"c:\old\destination";
            // Use c:\old\destination as the initial path to ensure that the default path for an existing directory is used
            _ = this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(@"c:\old\destination")).Returns(() => (true, @"c:\new\destination"));

            this.ViewModelInstance.AddEditDestination();

            Assert.Equal(@"c:\new\destination", this.ViewModelInstance.Model.Destination);
        }

        [Fact]
        public void AddEditDestination_DestinationDirectoryDoesNotExist()
        {
            this.ViewModelInstance!.Model.Destination = @"c:\old\destination";
            // Use C:\ as the initial path to ensure that the default path for a non-existent directory is used
            _ = this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(@"C:\")).Returns(() => (true, @"c:\new\destination"));

            this.ViewModelInstance.AddEditDestination();

            Assert.Equal(@"c:\new\destination", this.ViewModelInstance.Model.Destination);
        }

        [Fact]
        public void AddEditDestination_NoAction()
        {
            this.ViewModelInstance!.Model.Destination = @"c:\old\destination";
            // Use C:\ as the initial path to ensure that the default path for a non-existent directory is used
            _ = this.DialogServiceMock.Setup(s => s.ShowOpenFolderDialog(@"C:\")).Returns(() => (false, @"c:\new\destination"));

            this.ViewModelInstance.AddEditDestination();

            Assert.Equal(@"c:\old\destination", this.ViewModelInstance.Model.Destination);
        }

        [Fact]
        public void GetOpenFolderDialogInitialPath_DirectoryExists()
        {
            this.FileSystemMock.AddDirectory(@"c:\test");

            string result = this.ViewModelInstance!.GetOpenFolderDialogInitialPath(@"c:\test");

            Assert.Equal(@"c:\test", result);
        }

        [Fact]
        public void GetOpenFolderDialogInitialPath_DirectoryDoesNotExist()
        {
            string result = this.ViewModelInstance!.GetOpenFolderDialogInitialPath(@"c:\doesNotExist");

            Assert.Equal(Path.GetPathRoot(Environment.SystemDirectory), result);
        }
    }
}
