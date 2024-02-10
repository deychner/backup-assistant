using BackupAssistant.ViewModels;
using System.Collections.Specialized;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTest : MainWindowViewModelTestBase
    {
        public MainWindowViewModelTest() : base(false) { }

        [Fact]
        public void Constructor_InitializeFilters()
        {
            this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            // Verify new collection was created
            Assert.NotNull(this.SettingsServiceMock.Object.Filters);
            Assert.Empty(instance.Model.Filters);
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

            this.SettingsServiceMock.SetupProperty(f => f.Filters, filters);
            this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            // Verify count
            Assert.Equal(2, instance.Model.Filters.Count);

            // Verify contents
            Assert.Equal(filters[0], instance.Model.Filters[0]);
            Assert.Equal(filters[1], instance.Model.Filters[1]);
        }

        [Fact]
        public void Constructor_LoadSource_Valid()
        {
            this.FileSystemMock.AddDirectory(@"c:\source");

            this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            this.SettingsServiceMock.SetupProperty(s => s.Source, @"c:\source");
            this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            Assert.Equal(@"c:\source", instance.Model.Source);
        }

        [Fact]
        public void Constructor_LoadSource_Invalid()
        {
            this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            this.SettingsServiceMock.SetupProperty(s => s.Source, @"c:\source");
            this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            Assert.Equal(string.Empty, instance.Model.Source);
        }

        [Fact]
        public void Constructor_LoadDestination_Valid()
        {
            this.FileSystemMock.AddDirectory(@"c:\destination");

            this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            this.SettingsServiceMock.SetupProperty(d => d.Destination, @"c:\destination");
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            Assert.Equal(@"c:\destination", instance.Model.Destination);
        }

        [Fact]
        public void Constructor_LoadDestination_Invalid()
        {
            this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            this.SettingsServiceMock.SetupProperty(d => d.Destination, @"c:\destination");
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 0);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            Assert.Equal(string.Empty, instance.Model.Destination);
        }

        [Fact]
        public void Constructor_LoadBackupType()
        {
            this.SettingsServiceMock.SetupProperty(f => f.Filters, null);
            this.SettingsServiceMock.SetupProperty(s => s.Source, null);
            this.SettingsServiceMock.SetupProperty(d => d.Destination, null);
            this.SettingsServiceMock.SetupProperty(b => b.BackupType, 1);
            this.SettingsServiceMock.Setup(s => s.Save()).Verifiable();

            MainWindowViewModel instance = new(this.SettingsServiceMock.Object, this.DialogServiceMock.Object, this.LogServiceMock.Object, this.FileSystemMock);

            Assert.Equal(1, (int)instance.Model.BackupType);
        }
    }
}
