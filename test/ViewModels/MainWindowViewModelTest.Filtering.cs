using BackupAssistant.DataModels;
using BackupAssistant.ViewModels;
using Moq;
using System.Collections.ObjectModel;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestFiltering : MainWindowViewModelTestBase
    {
        [Fact]
        public void FilterImageSource()
        {
            this.ViewModelInstance!.Model.Filters = [];
            Assert.Equal("/assets/filter.png", this.ViewModelInstance.FilterImageSource);

            this.ViewModelInstance!.Model.Filters = ["test"];
            Assert.Equal("/assets/filter_apply.png", this.ViewModelInstance.FilterImageSource);
        }

        [Fact]
        public void EditFilters()
        {
            this.DialogServiceMock.Setup(s => s.ShowDialog<FilterSelection>(It.IsAny<IDialogViewModel>())).Returns(true);

            Mock<IDialogViewModel> dialogViewModelMock = new(MockBehavior.Strict);
            dialogViewModelMock.SetupSet(i => i.Input = It.IsAny<FilterSelectionInput>()).Verifiable();
            dialogViewModelMock.SetupGet(o => o.Output).Returns(new ObservableCollection<string> { "edited_filter" });

            // Add existing information
            this.ViewModelInstance!.Model.Source = "original";
            this.ViewModelInstance.Model.Filters = ["original_filter"];
            this.SettingsServiceMock.Object.Filters = ["original_filter"];

            this.ViewModelInstance.EditFilters(dialogViewModelMock.Object);

            // Verify results
            Assert.Single(this.ViewModelInstance.Model.Filters);
            Assert.Equal("edited_filter", this.ViewModelInstance.Model.Filters[0]);
            Assert.Single(this.SettingsServiceMock.Object.Filters);
            Assert.Equal("edited_filter", this.SettingsServiceMock.Object.Filters[0]);
        }

        [Fact]
        public void EditFilters_NoAction()
        {
            this.DialogServiceMock.Setup(s => s.ShowDialog<FilterSelection>(It.IsAny<IDialogViewModel>())).Returns(false);

            Mock<IDialogViewModel> dialogViewModelMock = new(MockBehavior.Strict);
            dialogViewModelMock.SetupSet(i => i.Input = It.IsAny<FilterSelectionInput>()).Verifiable();

            // Add existing information
            this.ViewModelInstance!.Model.Source = "original";
            this.ViewModelInstance.Model.Filters = ["original_filter"];
            this.SettingsServiceMock.Object.Filters = ["original_filter"];

            this.ViewModelInstance.EditFilters(dialogViewModelMock.Object);

            // Verify results
            Assert.Single(this.ViewModelInstance.Model.Filters);
            Assert.Equal("original_filter", this.ViewModelInstance.Model.Filters[0]);
            Assert.Single(this.SettingsServiceMock.Object.Filters);
            Assert.Equal("original_filter", this.SettingsServiceMock.Object.Filters[0]);
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
