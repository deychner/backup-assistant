using BackupAssistant.DataModels;
using BackupAssistant.Test.ViewModels.Base;
using BackupAssistant.ViewModels;
using Moq;
using System.Collections.Specialized;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTest : MainWindowViewModelTestBase
    {
        public MainWindowViewModelTest() : base(false) { }

        [Fact]
        public void Constructor_InitializeFilters()
        {
            _ = this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            _ = this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            _ = this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            _ = this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save());

            MainWindowViewModel instance = new(
                this.BackupServiceMock.Object,
                this.SettingsServiceMock.Object,
                this.DialogServiceMock.Object,
                this.ApplicationServiceMock.Object,
                this.LoggerMock.Object,
                this.InMemoryFileSystem);

            // Verify new collection was created and persisted, since there is no existing filter
            // collection to load
            Assert.NotNull(this.SettingsServiceMock.Object.Filters);
            Assert.Empty(instance.Model.Filters);
            this.SettingsServiceMock.Verify(s => s.Save(), Times.Once);
        }

        [Fact]
        public void Constructor_LoadFilters()
        {
            StringCollection filters =
            [
                "filter1",
                "filter2",
                null
            ];

            _ = this.SettingsServiceMock.SetupProperty(f => f.Filters, filters);
            _ = this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            _ = this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            _ = this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save());

            MainWindowViewModel instance = new(
                this.BackupServiceMock.Object,
                this.SettingsServiceMock.Object,
                this.DialogServiceMock.Object,
                this.ApplicationServiceMock.Object,
                this.LoggerMock.Object,
                this.InMemoryFileSystem);

            // Verify count
            Assert.Equal(2, instance.Model.Filters.Count);

            // Verify contents
            Assert.Equal(filters[0], instance.Model.Filters[0]);
            Assert.Equal(filters[1], instance.Model.Filters[1]);

            // The existing filter collection did not need to be initialized, so settings should not
            // have been re-saved just from opening the app
            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }

        [Fact]
        public void Constructor_Load_BackupSettings()
        {
            _ = this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            _ = this.SettingsServiceMock.SetupProperty(s => s.Source, @"c:\source");
            _ = this.SettingsServiceMock.SetupProperty(d => d.Destination, @"c:\destination");
            _ = this.SettingsServiceMock.SetupProperty(b => b.BackupType, 1);
            this.SettingsServiceMock.Setup(s => s.Save());

            MainWindowViewModel instance = new(
                this.BackupServiceMock.Object,
                this.SettingsServiceMock.Object,
                this.DialogServiceMock.Object,
                this.ApplicationServiceMock.Object,
                this.LoggerMock.Object,
                this.InMemoryFileSystem);

            Assert.Equal(@"c:\source", instance.Model.Source);
            Assert.Equal(@"c:\destination", instance.Model.Destination);
            Assert.Equal(1, (int)instance.Model.BackupType);
        }

        [Fact]
        public void Constructor_ShortOverload_DelegatesToFullConstructor()
        {
            _ = this.SettingsServiceMock.SetupProperty(f => f.Filters, []);
            _ = this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            _ = this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            _ = this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save());

            MainWindowViewModel instance = new(
                this.BackupServiceMock.Object,
                this.SettingsServiceMock.Object,
                this.DialogServiceMock.Object,
                this.ApplicationServiceMock.Object,
                this.LoggerMock.Object);

            Assert.NotNull(instance);
        }

        [Fact]
        public void Constructor_DoesNotResaveExistingSourceDestinationOrBackupType()
        {
            _ = this.SettingsServiceMock.SetupProperty(f => f.Filters, []);
            _ = this.SettingsServiceMock.SetupProperty(s => s.Source, @"c:\source");
            _ = this.SettingsServiceMock.SetupProperty(d => d.Destination, @"c:\destination");
            _ = this.SettingsServiceMock.SetupProperty(b => b.BackupType, 1);
            this.SettingsServiceMock.Setup(s => s.Save());

            _ = new MainWindowViewModel(
                this.BackupServiceMock.Object,
                this.SettingsServiceMock.Object,
                this.DialogServiceMock.Object,
                this.ApplicationServiceMock.Object,
                this.LoggerMock.Object,
                this.InMemoryFileSystem);

            // Loading Source, Destination and BackupType from settings must not write them back out
            this.SettingsServiceMock.Verify(s => s.Save(), Times.Never);
        }
    }
}
