using BackupAssistant.Services;
using BackupAssistant.Test.Services.Base;

namespace BackupAssistant.Test.Services
{
    public class BackupServiceTestFull : BackupServiceTestBase
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

        private async Task<ICollection<string>> GetFileListAsync(ICollection<string> filters)
        {
            return await this.BackupServiceInstance.GetFileListAsync(@"c:\Source", filters, CancellationToken.None);
        }

        #endregion

        #region RunFullBackup

        [Fact]
        public async Task RunFullBackup_Basic()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            _ = this.FileSystemMock.Directory.CreateDirectory(@"c:\Destination");

            await RunFullBackup();

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Destination\file1.txt"));
        }

        [Fact]
        public async Task RunFullBackup_BackupDeleted()
        {
            this.FileSystemMock.AddDirectory(@"c:\Source");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file3.txt");

            await RunFullBackup();

            Assert.False(this.FileSystemMock.File.Exists(@"c:\Destination\file3.txt"));
        }

        private async Task RunFullBackup()
        {
            var progress = new Progress<BackupProgress>();
            await this.BackupServiceInstance.RunFullBackupAsync(@"c:\Source", @"c:\Destination", [], progress, CancellationToken.None);
        }

        #endregion
    }
}
