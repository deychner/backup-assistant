using BackupAssistant.Test.ViewModels.Base;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestFiles : MainWindowViewModelTestBase
    {
        #region Source / Destination setters

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
        public void Source_Setter_KeepsFilters_WhenUnchanged()
        {
            this.ViewModelInstance!.Model.Source = @"c:\source";
            this.ViewModelInstance.Model.Filters = ["...\\keep"];

            this.ViewModelInstance.Source = @"c:\source";

            _ = Assert.Single(this.ViewModelInstance.Model.Filters);
        }

        [Fact]
        public void Source_Setter_DoesNotClearFilters_WhenInitializedFromEmpty()
        {
            // _model.Source starts as string.Empty, so the first real assignment must not clear
            // filters, since there is nothing to clear yet and this simulates startup loading.
            this.ViewModelInstance!.FilterItems.Add("somefilter");

            this.ViewModelInstance.Source = @"c:\new\source";

            _ = Assert.Single(this.ViewModelInstance.FilterItems);
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

        #endregion

        #region Folder picker

        [Fact]
        public async Task AddEditSource()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync(@"c:\new\source");

            this.ViewModelInstance!.Model.Source = @"c:\old\source";

            await this.ViewModelInstance.AddEditSourceAsync();

            Assert.Equal(@"c:\new\source", this.ViewModelInstance.Model.Source);
            Assert.Equal(@"c:\new\source", this.SettingsServiceMock.Object.Source);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddEditSource_ResetsFiltersWhenSourceChanges()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync(@"c:\new\source");

            this.ViewModelInstance!.Model.Source = @"c:\old\source";
            this.ViewModelInstance.Model.Filters = ["...\\stale"];

            await this.ViewModelInstance.AddEditSourceAsync();

            // Filters describe folders under the old source, so they cannot carry over
            Assert.Empty(this.ViewModelInstance.Model.Filters);
            Assert.Empty(this.SettingsServiceMock.Object.Filters);
        }

        [Fact]
        public async Task AddEditSource_Cancelled()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync((string?)null);

            this.ViewModelInstance!.Model.Source = @"c:\old\source";

            await this.ViewModelInstance.AddEditSourceAsync();

            Assert.Equal(@"c:\old\source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public async Task AddEditDestination()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync(@"c:\new\destination");

            this.ViewModelInstance!.Model.Destination = @"c:\old\destination";

            await this.ViewModelInstance.AddEditDestinationAsync();

            Assert.Equal(@"c:\new\destination", this.ViewModelInstance.Model.Destination);
            Assert.Equal(@"c:\new\destination", this.SettingsServiceMock.Object.Destination);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddEditDestination_Cancelled()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync((string?)null);

            this.ViewModelInstance!.Model.Destination = @"c:\old\destination";

            await this.ViewModelInstance.AddEditDestinationAsync();

            Assert.Equal(@"c:\old\destination", this.ViewModelInstance.Model.Destination);
        }

        #endregion

        #region Commands

        [Fact]
        public async Task AddEditSourceCommand()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync(@"c:\new\source");

            await this.ViewModelInstance!.AddEditSourceCommand.ExecuteAsync(null);

            Assert.Equal(@"c:\new\source", this.ViewModelInstance.Model.Source);
        }

        [Fact]
        public async Task AddEditDestinationCommand()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync(@"c:\new\destination");

            await this.ViewModelInstance!.AddEditDestinationCommand.ExecuteAsync(null);

            Assert.Equal(@"c:\new\destination", this.ViewModelInstance.Model.Destination);
        }

        [Fact]
        public async Task AddEditSource_Commands_BecomeExecutable()
        {
            _ = this.DialogServiceMock.Setup(s => s.ShowFolderPickerAsync()).ReturnsAsync(@"c:\source");

            Assert.False(this.ViewModelInstance!.EditFiltersCommand.CanExecute(null));

            await this.ViewModelInstance.AddEditSourceAsync();

            Assert.True(this.ViewModelInstance.EditFiltersCommand.CanExecute(null));
        }

        #endregion
    }
}
