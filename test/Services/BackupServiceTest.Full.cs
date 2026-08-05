using BackupAssistant.Services;
using BackupAssistant.Test.Services.Base;
using BackupAssistant.Test.TestHelpers;

namespace BackupAssistant.Test.Services
{
    public class BackupServiceTestFull : BackupServiceTestBase
    {
        #region GetFileList

        [Fact]
        public async Task GetFileList_SingleLevel()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file1.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file2.txt");

            ICollection<string> fileList = await GetFileListAsync();

            Assert.Equal(2, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\file2.txt", fileList);
        }

        [Fact]
        public async Task GetFileList_MultiLevel()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file1.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\L1F1\file2.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\L1F1\L2F1\file4.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\L1F2\file3.txt");

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
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file1.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\L1F1\file2.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\L1F1\L2F1\file4.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\L1F2\file3.txt");

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
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file1.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            await RunFullBackup();

            Assert.True(this.InMemoryFileSystem?.File.Exists(@"c:\Destination\file1.txt"));
        }

        [Fact]
        public async Task RunFullBackup_BackupDeleted()
        {
            this.InMemoryFileSystem?.AddDirectory(@"c:\Source");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\file3.txt");

            await RunFullBackup();

            Assert.False(this.InMemoryFileSystem?.File.Exists(@"c:\Destination\file3.txt"));
        }

        private async Task RunFullBackup()
        {
            var progress = new Progress<BackupProgress>();
            await this.BackupServiceInstance.RunFullBackupAsync(@"c:\Source", @"c:\Destination", [], progress, CancellationToken.None);
        }

        [Fact]
        public async Task RunFullBackupAsync_NullProgress_DoesNotThrow()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file1.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            await this.BackupServiceInstance.RunFullBackupAsync(@"c:\Source", @"c:\Destination", [], null!, CancellationToken.None);

            Assert.True(this.InMemoryFileSystem?.File.Exists(@"c:\Destination\file1.txt"));
        }

        [Fact]
        public async Task RunFullBackup_ProgressReachesOneHundred_DespiteConcurrentWorkers()
        {
            for (int i = 0; i < 25; i++)
            {
                this.InMemoryFileSystem?.AddEmptyFile($@"c:\Source\file{i}.txt");
            }
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            List<BackupProgress> reports = [];

            SynchronizationContext? original = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new InlineSynchronizationContext());
            try
            {
                IProgress<BackupProgress> progress = new Progress<BackupProgress>(reports.Add);
                await this.BackupServiceInstance.RunFullBackupAsync(@"c:\Source", @"c:\Destination", [], progress, CancellationToken.None);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(original);
            }

            BackupProgress lastProgressReport = reports.Last(r => r.Progress.HasValue);
            Assert.Equal(100, lastProgressReport.Progress);
            Assert.Equal("Backup is complete.", reports[^1].Status);
        }

        #endregion
    }
}
