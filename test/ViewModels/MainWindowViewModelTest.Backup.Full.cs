using BackupAssistant.Test.ViewModels.Base;
using System.Collections.ObjectModel;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackupFull : MainWindowViewModelTestBase
    {
        #region GetFileList

        [Fact]
        public async Task GetFileList_SingleLevel()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file2.txt");

            ICollection<string> fileList = await GetFileListAsync();

            Assert.Equal(2, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\file2.txt", fileList);
        }

        [Fact]
        public async Task GetFileList_MultiLevel()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\file2.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\L2F1\file4.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F2\file3.txt");

            ICollection<string> fileList = await GetFileListAsync();

            Assert.Equal(4, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\file2.txt", fileList);
            Assert.Contains(@"c:\Source\L1F2\file3.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\L2F1\file4.txt", fileList);
        }

        [Fact]
        public async Task GetFileList_MultiLevel_Filters()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\file2.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\L2F1\file4.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F2\file3.txt");

            ICollection<string> fileList = await GetFileListAsync([@"...\L1F1"]);

            Assert.Equal(3, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\file2.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\L2F1\file4.txt", fileList);
        }

        private async Task<ICollection<string>> GetFileListAsync()
        {
            return await GetFileListAsync([]);
        }

        private async Task<ICollection<string>> GetFileListAsync(ObservableCollection<string> filters)
        {
            this.ViewModelInstance!.Model.Filters = filters;

            return await this.ViewModelInstance!.GetFileListAsync(@"c:\Source", CancellationToken.None);
        }

        #endregion

        #region RunFullBackupInternal

        [Fact]
        public async Task RunFullBackupInternal_Basic()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");

            await RunFullBackupInternal();

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Destination\file1.txt"));
        }

        [Fact]
        public async Task RunFullBackupInternal_BackupDeleted()
        {
            this.FileSystemMock.AddDirectory(@"c:\Source");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file3.txt");

            await RunFullBackupInternal();

            Assert.False(this.FileSystemMock.File.Exists(@"c:\Destination\file3.txt"));
        }

        private async Task RunFullBackupInternal()
        {
            this.ViewModelInstance!.Model.Source = @"c:\Source";
            this.ViewModelInstance.Model.Destination = @"c:\Destination";
            this.ViewModelInstance.Model.Filters = [];

            await this.ViewModelInstance.RunFullBackupInternalAsync(CancellationToken.None);
        }

        #endregion
    }
}
