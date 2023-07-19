using BackupAssistant.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly AsyncRelayCommand _runBackupCommand;
        public IAsyncRelayCommand RunBackupCommand => _runBackupCommand;

        public ICommand CancelRunBackupCommand => this.RunBackupCommand.CreateCancelCommand();

        public async Task RunBackup(CancellationToken token)
        {
            try
            {
                await Task.Run(() => RunBackupInternal(token), token);
            }
            catch (OperationCanceledException)
            {
                this.Status = "Backup was canceled.";
            }
            finally
            {
                _logService.WriteLogEntry();
            }
        }

        public bool CanRunBackup()
        {
            return !string.IsNullOrEmpty(this.Source) && !string.IsNullOrEmpty(this.Destination) && !_runBackupCommand.IsRunning;
        }

        internal void RunBackupInternal(CancellationToken token)
        {
            switch (this.BackupType)
            {
                case BackupType.Full:
                    RunFullBackupInternal(token);
                    break;
                case BackupType.Incremental:
                    RunIncrementalBackupInternal(token);
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        public BackupType BackupType
        {
            get => _model.BackupType;
            set
            {
                _model.BackupType = value;
                OnPropertyChanged(nameof(BackupType));

                // Update settings
                _settingsService.BackupType = (int)_model.BackupType;
                _settingsService.Save();
            }
        }

        #region Safety

        private IReadOnlyCollection<string> SafeGetFiles(string directory)
        {
            try
            {
                string[] files = _fileSystem.Directory.GetFiles(directory);
                return new ReadOnlyCollection<string>(files);
            }
            catch
            {
                _logService.AddToLogEntry($"Failed to get files in directory '{directory}'.");

                return new ReadOnlyCollection<string>(Array.Empty<string>());
            }
        }

        private IReadOnlyCollection<string> SafeGetDirectories(string directory)

        {
            try
            {
                string[] directories = _fileSystem.Directory.GetDirectories(directory);
                return new ReadOnlyCollection<string>(directories);
            }
            catch
            {
                _logService.AddToLogEntry($"Failed to get directories in directory '{directory}'.");

                return new ReadOnlyCollection<string>(Array.Empty<string>());
            }
        }

        public void SafeCopyFile(string sourceFileName, string destinationFileName)
        {
            SafeCopyFile(sourceFileName, destinationFileName, false);
        }

        public void EnsureDirectoryPathExists(string path)
        {
            string? directory = _fileSystem.Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !_fileSystem.Directory.Exists(directory))
            {
                _fileSystem.Directory.CreateDirectory(directory);
            }
        }

        public void SafeCopyFile(string sourceFileName, string destinationFileName, bool overwrite)
        {
            try
            {
                EnsureDirectoryPathExists(destinationFileName);

                _fileSystem.File.Copy(sourceFileName, destinationFileName, overwrite);
            }
            catch
            {
                _logService.AddToLogEntry($"Copy file failed for file '{sourceFileName}'.");
            }
        }

        #endregion

        #region Compression

        private static string GetFullFileName(string abbreviatedFileName, string lengthenString)
        {
            return abbreviatedFileName.Replace("...", lengthenString);
        }

        #endregion
    }
}
