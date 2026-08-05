using BackupAssistant.DataModels;
using BackupAssistant.Test.ViewModels.Base;
using BackupAssistant.ViewModels;
using Moq;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTest : MainWindowViewModelTestBase
    {
        public MainWindowViewModelTest() : base(false) { }

        [Fact]
        public void Constructor_NoSavedFilters()
        {
            SetupSettings([], source: null, destination: null, backupType: 0);

            MainWindowViewModel instance = CreateViewModel();

            Assert.Empty(instance.Model.Filters);
            Assert.Equal("All folders", instance.FilterSummary);
            Assert.False(instance.HasFilters);
        }

        [Fact]
        public void Constructor_LoadFilters()
        {
            // The blank entry stands in for a settings file that has been hand-edited or
            // written by an older version; it must not become a filter.
            SetupSettings(["filter1", "filter2", ""], source: null, destination: null, backupType: 0);

            MainWindowViewModel instance = CreateViewModel();

            Assert.Equal(2, instance.Model.Filters.Count);
            Assert.Equal("filter1", instance.Model.Filters[0]);
            Assert.Equal("filter2", instance.Model.Filters[1]);
            Assert.True(instance.HasFilters);
            Assert.Equal("2 folders selected", instance.FilterSummary);
        }

        [Fact]
        public void Constructor_Load_BackupSettings()
        {
            SetupSettings([], @"c:\source", @"c:\destination", (int)BackupType.Incremental);

            MainWindowViewModel instance = CreateViewModel();

            Assert.Equal(@"c:\source", instance.Model.Source);
            Assert.Equal(@"c:\destination", instance.Model.Destination);
            Assert.Equal(BackupType.Incremental, instance.Model.BackupType);
        }

        [Fact]
        public void Constructor_DoesNotRewriteSettings()
        {
            SetupSettings([], @"c:\source", @"c:\destination", (int)BackupType.Full);

            _ = CreateViewModel();

            // Merely starting up must not touch the settings file.
            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }

        private void SetupSettings(IList<string> filters, string? source, string? destination, int backupType)
        {
            _ = this.SettingsServiceMock.SetupProperty(f => f.Filters, filters);
            _ = this.SettingsServiceMock.SetupProperty(s => s.Source, source);
            _ = this.SettingsServiceMock.SetupProperty(d => d.Destination, destination);
            _ = this.SettingsServiceMock.SetupProperty(b => b.BackupType, backupType);
            this.SettingsServiceMock.Setup(s => s.Save());
        }
    }
}
