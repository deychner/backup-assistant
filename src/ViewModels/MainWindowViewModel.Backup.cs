using BackupAssistant.DataModels;
using BackupAssistant.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupAssistant.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private const string FilePathAbbreviation = "...";

        private AsyncRelayCommand? _runBackupCommand;
        public IAsyncRelayCommand RunBackupCommand => _runBackupCommand ??= new AsyncRelayCommand(async (CancellationToken token) => await RunBackupAsync(token), CanRunBackup);

        public ICommand CancelRunBackupCommand => this.RunBackupCommand.CreateCancelCommand();

        public async Task RunBackupAsync(CancellationToken token)
        {
            _logService.ClearLog();

            if (!_fileSystem.Directory.Exists(this.Source))
            {
                _logService.AddToLogEntry($"Backup failed. The source directory '{this.Source}' does not exist.");

                this.Status = "The source directory does not exist.";
                return;
            }

            if (!_fileSystem.Directory.Exists(this.Destination))
            {
                _logService.AddToLogEntry($"Backup failed. The destination directory '{this.Destination}' does not exist.");

                this.Status = "The destination directory does not exist.";
                return;
            }

            try
            {
                switch (this.BackupType)
                {
                    case BackupType.Full:
                        await Task.Run(() => RunFullBackupInternal(token), token);
                        break;
                    case BackupType.Incremental:
                        await Task.Run(async () => await RunIncrementalBackupInternalAsync(token), token);
                        break;
                    default:
                        // do nothing
                        break;
                }
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
            return !string.IsNullOrEmpty(this.Source) && !string.IsNullOrEmpty(this.Destination) && !_runBackupCommand!.IsRunning;
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

        public int Progress
        {
            get { return _model.Progress; }
            set
            {
                if (value < 0)
                {
                    _model.Progress = 0;
                }
                else if (value > 100)
                {
                    _model.Progress = 100;
                }
                else
                {
                    _model.Progress = value;
                }

                OnPropertyChanged(nameof(Progress));
            }
        }

        public bool ProgressBarIsIndeterminate
        {
            get { return _model.ProgressBarIsIndeterminate; }
            set
            {
                _model.ProgressBarIsIndeterminate = value;
                OnPropertyChanged(nameof(ProgressBarIsIndeterminate));
            }
        }

        public string Status
        {
            get { return _model.Status; }
            set
            {
                _model.Status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        #region Safety

        public IFileInfo? SafeGetFileInfo(string file)
        {
            try
            {
                return _fileSystem.FileInfo.New(file);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Failed to get file information for file '{file}', Exception: {e.Message}");

                return null;
            }
        }

        public IReadOnlyCollection<string> SafeGetFiles(string directory)
        {
            try
            {
                string[] files = _fileSystem.Directory.GetFiles(directory);
                return new ReadOnlyCollection<string>(files);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Failed to get files in directory '{directory}', Exception: {e.Message}");

                return new ReadOnlyCollection<string>([]);
            }
        }

        public IReadOnlyCollection<string> SafeGetDirectories(string directory)

        {
            try
            {
                string[] directories = _fileSystem.Directory.GetDirectories(directory);
                return new ReadOnlyCollection<string>(directories);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Failed to get directories in directory '{directory}', Exception: {e.Message}");

                return new ReadOnlyCollection<string>([]);
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
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Copy file failed for file '{sourceFileName}', Exception: {e.Message}");
            }
        }

        public async Task SafeCopyFileAsync(string sourceFileName, string destinationFileName, bool overwrite)
        {
            try
            {
                EnsureDirectoryPathExists(destinationFileName);

                await Task.Run(() => _fileSystem.File.Copy(sourceFileName, destinationFileName, overwrite));
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Copy file failed for file '{sourceFileName}', Exception: {e.Message}");
            }
        }

        public void SafeDeleteFile(string file)
        {
            try
            {
                _fileSystem.File.Delete(file);
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Delete file failed for file '{file}', Exception: {e.Message}");
            }
        }

        public async Task SafeDeleteFileAsync(string file)
        {
            try
            {
                await Task.Run(() => _fileSystem.File.Delete(file));
            }
            catch (Exception e)
            {
                _logService.AddToLogEntry($"Delete file failed for file '{file}', Exception: {e.Message}");
            }
        }

        #endregion

        #region Compression

        string ShrinkSourceFileName(string fileName)
        {
            return GetAbbreviatedFileName(fileName, this.Source);
        }

        string ShrinkDestinationFileName(string fileName)
        {
            return GetAbbreviatedFileName(fileName, this.Destination);
        }

        string ExpandSourceFileName(string fileName)
        {
            return GetFullFileName(fileName, this.Source);
        }

        string ExpandDestinationFileName(string fileName)
        {
            return GetFullFileName(fileName, this.Destination);
        }

        static string GetFullFileName(string abbreviatedFileName, string prefix)
        {
            return abbreviatedFileName.ReplaceFirst(FilePathAbbreviation, prefix).Replace(@"\\", @"\");
        }

        static string GetAbbreviatedFileName(string fileName, string prefix)
        {
            // Ensure all shortened file names start with "...\"
            if (prefix.EndsWith('\\'))
            {
                return fileName.ReplaceFirst(prefix, $@"{FilePathAbbreviation}\");
            }
            else
            {
                return fileName.ReplaceFirst(prefix, FilePathAbbreviation);
            }
        }

        #endregion
    }
}
