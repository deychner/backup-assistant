using BackupAssistant.DataModels;
using BackupAssistant.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.Services
{
    public class JsonSettingsServiceTest
    {
        private const string SettingsPath = @"c:\settings\settings.json";

        private readonly MockFileSystem _fileSystem = new();
        private readonly Mock<ILogger<JsonSettingsService>> _loggerMock = new(MockBehavior.Loose);

        [Fact]
        public void Defaults_WhenNoFileExists()
        {
            JsonSettingsService instance = Create();

            Assert.Equal(string.Empty, instance.Source);
            Assert.Equal(string.Empty, instance.Destination);
            Assert.Equal((int)BackupType.Incremental, instance.BackupType);
            Assert.Empty(instance.Filters);
        }

        [Fact]
        public void Save_ThenReload_RoundTripsEveryValue()
        {
            JsonSettingsService instance = Create();

            instance.Source = @"c:\source";
            instance.Destination = @"d:\backup";
            instance.BackupType = (int)BackupType.Full;
            instance.Filters = [@"...\photos", @"...\music"];
            instance.Save();

            JsonSettingsService reloaded = Create();

            Assert.Equal(@"c:\source", reloaded.Source);
            Assert.Equal(@"d:\backup", reloaded.Destination);
            Assert.Equal((int)BackupType.Full, reloaded.BackupType);
            Assert.Equal([@"...\photos", @"...\music"], reloaded.Filters);
        }

        [Fact]
        public void Save_CreatesMissingDirectory()
        {
            JsonSettingsService instance = Create();

            instance.Source = @"c:\source";
            instance.Save();

            Assert.True(_fileSystem.File.Exists(SettingsPath));
        }

        [Fact]
        public void Save_WritesReadableJson()
        {
            JsonSettingsService instance = Create();

            instance.Source = @"c:\source";
            instance.Save();

            string json = _fileSystem.File.ReadAllText(SettingsPath);

            // Indented output, so a user can inspect or hand-edit the file
            Assert.Contains("\"Source\"", json);
            Assert.Contains(Environment.NewLine, json);
        }

        [Fact]
        public void Load_FallsBackToDefaults_WhenFileIsCorrupt()
        {
            _fileSystem.AddFile(SettingsPath, new MockFileData("this is not json"));

            JsonSettingsService instance = Create();

            Assert.Equal(string.Empty, instance.Source);
            Assert.Equal((int)BackupType.Incremental, instance.BackupType);
            VerifyWarningLogged();
        }

        [Fact]
        public void Load_FallsBackToDefaults_WhenFileIsJsonNull()
        {
            _fileSystem.AddFile(SettingsPath, new MockFileData("null"));

            JsonSettingsService instance = Create();

            Assert.Equal(string.Empty, instance.Source);
            Assert.Empty(instance.Filters);
        }

        [Fact]
        public void Load_Tolerates_PartialSettingsFile()
        {
            _fileSystem.AddFile(SettingsPath, new MockFileData("{ \"Source\": \"c:\\\\only-source\" }"));

            JsonSettingsService instance = Create();

            Assert.Equal(@"c:\only-source", instance.Source);
            Assert.Equal(string.Empty, instance.Destination);
            Assert.Equal((int)BackupType.Incremental, instance.BackupType);
        }

        [Fact]
        public void Save_LogsAndSwallows_WhenWriteFails()
        {
            // A directory occupying the settings path makes the write fail
            _fileSystem.AddDirectory(SettingsPath);

            JsonSettingsService instance = Create();
            instance.Source = @"c:\source";

            instance.Save();

            VerifyWarningLogged();
        }

        [Fact]
        public void DefaultFilePath_IsUnderLocalApplicationData()
        {
            string path = JsonSettingsService.GetDefaultFilePath(_fileSystem);

            Assert.StartsWith(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                path);
            Assert.Contains(JsonSettingsService.CompanyFolderName, path);
            Assert.EndsWith(JsonSettingsService.SettingsFileName, path);
        }

        [Fact]
        public void DefaultConstructor_UsesDefaultFilePath()
        {
            // Exercises the production constructor overload without touching the real file system
            JsonSettingsService instance = new(_fileSystem, _loggerMock.Object);

            Assert.Equal(string.Empty, instance.Source);
        }

        private JsonSettingsService Create() => new(_fileSystem, _loggerMock.Object, SettingsPath);

        private void VerifyWarningLogged()
        {
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }
    }
}
