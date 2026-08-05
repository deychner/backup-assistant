using BackupAssistant.Test.ViewModels.Base;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestFiles : MainWindowViewModelTestBase
    {
        [Fact]
        public void Source_Setter_SavesSettingsAndClearsFilters_WhenChanged()
        {
            this.ViewModelInstance!.Model.Source = @"c:\old\source";
            this.ViewModelInstance.FilterItems.Add("somefilter");

            this.ViewModelInstance.Source = @"c:\new\source";

            Assert.Equal(@"c:\new\source", this.ViewModelInstance.Source);
            Assert.Empty(this.ViewModelInstance.FilterItems);
            this.SettingsServiceMock.VerifySet(s => s.Source = @"c:\new\source", Times.Once);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void Source_Setter_DoesNotSaveSettings_WhenUnchanged()
        {
            this.ViewModelInstance!.Model.Source = @"c:\same";

            this.ViewModelInstance.Source = @"c:\same";

            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }

        [Fact]
        public void Source_Setter_DoesNotClearFilters_WhenInitializedFromEmpty()
        {
            // _model.Source starts as string.Empty, so the first real assignment must not clear
            // filters, since there is nothing to clear yet and this simulates startup loading.
            this.ViewModelInstance!.FilterItems.Add("somefilter");

            this.ViewModelInstance.Source = @"c:\new\source";

            Assert.Single(this.ViewModelInstance.FilterItems);
        }

        [Fact]
        public void Destination_Setter_SavesSettings_WhenChanged()
        {
            this.ViewModelInstance!.Destination = @"c:\new\destination";

            this.SettingsServiceMock.VerifySet(s => s.Destination = @"c:\new\destination", Times.Once);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void Destination_Setter_DoesNotSaveSettings_WhenUnchanged()
        {
            this.ViewModelInstance!.Model.Destination = @"c:\same";

            this.ViewModelInstance.Destination = @"c:\same";

            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }

        [Fact]
        public void AddEditSource()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\old\source");

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
            this.InMemoryFileSystem.AddDirectory(@"c:\old\destination");

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
            this.InMemoryFileSystem.AddDirectory(@"c:\test");

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
