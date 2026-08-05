using BackupAssistant.Test.ViewModels.Base;
using BackupAssistant.ViewModels;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestFiltering : MainWindowViewModelTestBase
    {
        [Theory]
        [InlineData(0, false, "All folders")]
        [InlineData(1, true, "1 folder selected")]
        [InlineData(3, true, "3 folders selected")]
        public void FilterSummary(int filterCount, bool expectedHasFilters, string expectedSummary)
        {
            this.ViewModelInstance!.Model.Filters = [.. Enumerable.Range(0, filterCount).Select(i => $"...\\dir{i}")];

            Assert.Equal(expectedHasFilters, this.ViewModelInstance.HasFilters);
            Assert.Equal(expectedSummary, this.ViewModelInstance.FilterSummary);
        }

        [Fact]
        public async Task EditFilters()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source\keep");
            this.InMemoryFileSystem.AddDirectory(@"c:\source\skip");

            FilterSelectionViewModel dialogViewModel = new(this.InMemoryFileSystem);

            // Stand in for the user ticking one folder and pressing Apply
            _ = this.DialogServiceMock
                .Setup(s => s.ShowFilterSelectionDialogAsync(dialogViewModel))
                .Callback(() => dialogViewModel.FilterItems.First(f => f.Path == @"...\keep").IsChecked = true)
                .ReturnsAsync(true);

            this.ViewModelInstance!.Model.Source = @"c:\source";
            this.ViewModelInstance.Model.Filters = ["...\\original"];
            this.SettingsServiceMock.Object.Filters = ["...\\original"];

            await this.ViewModelInstance.EditFiltersAsync(dialogViewModel);

            Assert.Equal([@"...\keep"], this.ViewModelInstance.Model.Filters);
            Assert.Equal([@"...\keep"], this.SettingsServiceMock.Object.Filters);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task EditFilters_Cancelled()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source\keep");

            FilterSelectionViewModel dialogViewModel = new(this.InMemoryFileSystem);

            _ = this.DialogServiceMock
                .Setup(s => s.ShowFilterSelectionDialogAsync(dialogViewModel))
                .ReturnsAsync(false);

            this.ViewModelInstance!.Model.Source = @"c:\source";
            this.ViewModelInstance.Model.Filters = ["...\\original"];
            this.SettingsServiceMock.Object.Filters = ["...\\original"];

            await this.ViewModelInstance.EditFiltersAsync(dialogViewModel);

            Assert.Equal([@"...\original"], this.ViewModelInstance.Model.Filters);
            Assert.Equal([@"...\original"], this.SettingsServiceMock.Object.Filters);
        }

        [Fact]
        public async Task EditFilters_SeedsDialogWithCurrentSelection()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source\keep");
            this.InMemoryFileSystem.AddDirectory(@"c:\source\skip");

            FilterSelectionViewModel dialogViewModel = new(this.InMemoryFileSystem);

            _ = this.DialogServiceMock
                .Setup(s => s.ShowFilterSelectionDialogAsync(dialogViewModel))
                .ReturnsAsync(false);

            this.ViewModelInstance!.Model.Source = @"c:\source";
            this.ViewModelInstance.Model.Filters = ["...\\keep"];

            await this.ViewModelInstance.EditFiltersAsync(dialogViewModel);

            // The already-selected folder should arrive at the dialog pre-checked
            Assert.True(dialogViewModel.FilterItems.First(f => f.Path == @"...\keep").IsChecked);
            Assert.False(dialogViewModel.FilterItems.First(f => f.Path == @"...\skip").IsChecked);
        }

        [Fact]
        public async Task EditFiltersCommand_BuildsItsOwnDialogViewModel()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\source\keep");

            _ = this.DialogServiceMock
                .Setup(s => s.ShowFilterSelectionDialogAsync(It.IsAny<FilterSelectionViewModel>()))
                .Callback((FilterSelectionViewModel vm) => vm.FilterItems.First().IsChecked = true)
                .ReturnsAsync(true);

            this.ViewModelInstance!.Model.Source = @"c:\source";

            await this.ViewModelInstance.EditFiltersCommand.ExecuteAsync(null);

            Assert.Equal([@"...\keep"], this.ViewModelInstance.Model.Filters);
        }

        [Fact]
        public void FilterItems_Setter_SavesSettings_WhenChanged()
        {
            this.ViewModelInstance!.FilterItems = ["new_filter"];

            _ = Assert.Single(this.SettingsServiceMock.Object.Filters);
            Assert.Equal("new_filter", this.SettingsServiceMock.Object.Filters[0]);
        }

        [Fact]
        public void CanEditFilters()
        {
            this.ViewModelInstance!.Model.Source = string.Empty;
            Assert.False(this.ViewModelInstance.CanEditFilters());

            this.ViewModelInstance!.Model.Source = @"c:\source";
            Assert.True(this.ViewModelInstance.CanEditFilters());
        }
    }
}
