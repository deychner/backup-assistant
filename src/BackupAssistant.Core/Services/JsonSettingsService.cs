using BackupAssistant.DataModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.Json;

namespace BackupAssistant.Services
{
    /// <summary>
    /// Persists user settings as JSON under the user's local application data folder.
    /// <para>
    /// This replaces the <c>System.Configuration.ApplicationSettingsBase</c> ("Settings.settings")
    /// mechanism used by the WPF version, which has no equivalent in WinUI 3. Going through
    /// <see cref="IFileSystem"/> also makes the whole class unit testable.
    /// </para>
    /// </summary>
    public class JsonSettingsService : ISettingsService
    {
        internal const string CompanyFolderName = "Anaheim_Electronics";
        internal const string SettingsFileName = "settings.json";

        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        private readonly IFileSystem _fileSystem;
        private readonly ILogger<JsonSettingsService> _logger;
        private readonly string _filePath;
        private readonly BackupSettings _settings;

        public JsonSettingsService(IFileSystem fileSystem, ILogger<JsonSettingsService> logger)
            : this(fileSystem, logger, GetDefaultFilePath(fileSystem))
        { }

        public JsonSettingsService(IFileSystem fileSystem, ILogger<JsonSettingsService> logger, string filePath)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _filePath = filePath;
            _settings = Load();
        }

        public int BackupType
        {
            get => _settings.BackupType;
            set => _settings.BackupType = value;
        }

        public string Destination
        {
            get => _settings.Destination;
            set => _settings.Destination = value;
        }

        public IList<string> Filters
        {
            get => _settings.Filters;
            set => _settings.Filters = value;
        }

        public string Source
        {
            get => _settings.Source;
            set => _settings.Source = value;
        }

        public void Save()
        {
            try
            {
                string? directory = _fileSystem.Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !_fileSystem.Directory.Exists(directory))
                {
                    _ = _fileSystem.Directory.CreateDirectory(directory);
                }

                _fileSystem.File.WriteAllText(_filePath, JsonSerializer.Serialize(_settings, SerializerOptions));
            }
            catch (Exception e)
            {
                // Failing to persist settings must never take the application down.
                _logger.LogWarning("Failed to save settings to '{filePath}', Exception: {exception}", _filePath, e.Message);
            }
        }

        /// <summary>
        /// Gets the full path of the settings file, alongside the application's log files.
        /// </summary>
        internal static string GetDefaultFilePath(IFileSystem fileSystem)
        {
            return fileSystem.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                CompanyFolderName,
                SettingsFileName);
        }

        private BackupSettings Load()
        {
            try
            {
                if (!_fileSystem.File.Exists(_filePath))
                {
                    // First run: start from the defaults.
                    return new BackupSettings();
                }

                string json = _fileSystem.File.ReadAllText(_filePath);

                return JsonSerializer.Deserialize<BackupSettings>(json, SerializerOptions) ?? new BackupSettings();
            }
            catch (Exception e)
            {
                // A corrupt or unreadable settings file should not stop the application from starting.
                _logger.LogWarning("Failed to read settings from '{filePath}'. Defaults will be used. Exception: {exception}", _filePath, e.Message);
                return new BackupSettings();
            }
        }
    }
}
