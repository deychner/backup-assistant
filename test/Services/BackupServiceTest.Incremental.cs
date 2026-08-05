using BackupAssistant.DataModels;
using BackupAssistant.Test.Services.Base;
using BackupAssistant.Test.TestHelpers;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.Services
{
    public class BackupServiceTestIncremental : BackupServiceTestBase
    {
        #region GetCombinedFileList

        [Fact]
        public async Task GetCombinedFileList_SingleLevel_SourceOnly()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            KeyValuePair<string, FileListing> kvp = Assert.Single(fileList);
            Assert.Equal(@"...\file.txt", kvp.Key);
            Assert.True(kvp.Value.IsInSource);
            Assert.False(kvp.Value.IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_SingleLevel_DestinationOnly()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\file.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Source");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            KeyValuePair<string, FileListing> kvp = Assert.Single(fileList);
            Assert.Equal(@"...\file.txt", kvp.Key);
            Assert.False(kvp.Value.IsInSource);
            Assert.True(kvp.Value.IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_SingleLevel_Both()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\file.txt");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            KeyValuePair<string, FileListing> kvp = Assert.Single(fileList);
            Assert.Equal(@"...\file.txt", kvp.Key);
            Assert.True(kvp.Value.IsInSource);
            Assert.True(kvp.Value.IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_MultiLevel_SourceOnly()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\SubDirectory\file.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            KeyValuePair<string, FileListing> kvp = Assert.Single(fileList);
            Assert.Equal(@"...\SubDirectory\file.txt", kvp.Key);
            Assert.True(kvp.Value.IsInSource);
            Assert.False(kvp.Value.IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_MultiLevel_DestinationOnly()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\SubDirectory\file.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Source");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            KeyValuePair<string, FileListing> kvp = Assert.Single(fileList);
            Assert.Equal(@"...\SubDirectory\file.txt", kvp.Key);
            Assert.False(kvp.Value.IsInSource);
            Assert.True(kvp.Value.IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_MultiLevel_Both()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\SubDirectory\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\SubDirectory\file.txt");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            KeyValuePair<string, FileListing> kvp = Assert.Single(fileList);
            Assert.Equal(@"...\SubDirectory\file.txt", kvp.Key);
            Assert.True(kvp.Value.IsInSource);
            Assert.True(kvp.Value.IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_Filters()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\Search\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\Ignore\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\Search\file.txt");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\Ignore\file.txt");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync([@"...\Search"]);

            Assert.Equal(2, fileList.Count);

            Assert.Contains(fileList.Keys, (f) => f == @"...\file.txt");
            Assert.True(fileList[@"...\file.txt"].IsInSource);
            Assert.True(fileList[@"...\file.txt"].IsInDestination);

            Assert.Contains(fileList.Keys, (f) => f == @"...\Search\file.txt");
            Assert.True(fileList[@"...\Search\file.txt"].IsInSource);
            Assert.True(fileList[@"...\Search\file.txt"].IsInDestination);
        }

        private async Task<IDictionary<string, FileListing>> GetCombinedFileListAsync()
        {
            return await GetCombinedFileListAsync([]);
        }

        private async Task<IDictionary<string, FileListing>> GetCombinedFileListAsync(ICollection<string> filters)
        {
            return await this.BackupServiceInstance.GetCombinedFileListAsync(@"c:\Source", @"c:\Destination", filters, CancellationToken.None);
        }

        #endregion

        #region RunIncrementalBackup

        [Fact]
        public async Task RunIncrementalBackup_NoAction()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            this.InMemoryFileSystem?.AddFile(@"c:\Source\file.txt", new MockFileData("Sample data") { LastWriteTime = now });
            this.InMemoryFileSystem?.AddFile(@"c:\Destination\file.txt", new MockFileData("Sample data") { LastWriteTime = now });

            await RunIncrementalBackup();

            // Check that the file was not touched
            Assert.True(now == this.InMemoryFileSystem?.FileInfo.New(@"c:\Destination\file.txt").LastWriteTime, "The file was updated.");
        }

        [Fact]
        public async Task RunIncrementalBackup_Copy()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            await RunIncrementalBackup();

            // Check that the file was copied
            Assert.True(this.InMemoryFileSystem?.FileExists(@"c:\Destination\file.txt"), "The file was not copied.");
        }

        [Fact]
        public async Task RunIncrementalBackup_Overwrite()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            this.InMemoryFileSystem?.AddFile(@"c:\Source\file.txt", new MockFileData("Sample data") { LastWriteTime = now });
            this.InMemoryFileSystem?.AddFile(@"c:\Destination\file.txt", new MockFileData("Sample data") { LastWriteTime = now.AddMinutes(-1) });

            await RunIncrementalBackup();

            // Check that the file was touched
            Assert.True(now == this.InMemoryFileSystem?.FileInfo.New(@"c:\Destination\file.txt").LastWriteTime, "The file was not updated.");
        }

        [Fact]
        public async Task RunIncrementalBackup_Delete()
        {
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Source");
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Destination\file.txt");

            await RunIncrementalBackup();

            // Check that the file was deleted
            Assert.False(this.InMemoryFileSystem?.FileExists(@"c:\Destination\file.txt"), "The file was not deleted.");
        }

        private async Task RunIncrementalBackup()
        {
            var progress = new Progress<BackupAssistant.Services.BackupProgress>();
            await this.BackupServiceInstance.RunIncrementalBackupAsync(@"c:\Source", @"c:\Destination", [], progress, CancellationToken.None);
        }

        [Fact]
        public async Task RunIncrementalBackupAsync_NullProgress_DoesNotThrow()
        {
            this.InMemoryFileSystem?.AddEmptyFile(@"c:\Source\file.txt");
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            await this.BackupServiceInstance.RunIncrementalBackupAsync(@"c:\Source", @"c:\Destination", [], null!, CancellationToken.None);

            Assert.True(this.InMemoryFileSystem?.FileExists(@"c:\Destination\file.txt"));
        }

        [Fact]
        public async Task RunIncrementalBackup_ProgressReachesOneHundred_DespiteConcurrentWorkers()
        {
            for (int i = 0; i < 25; i++)
            {
                this.InMemoryFileSystem?.AddEmptyFile($@"c:\Source\file{i}.txt");
            }
            _ = this.InMemoryFileSystem?.Directory.CreateDirectory(@"c:\Destination");

            List<BackupAssistant.Services.BackupProgress> reports = [];

            SynchronizationContext? original = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new InlineSynchronizationContext());
            try
            {
                IProgress<BackupAssistant.Services.BackupProgress> progress = new Progress<BackupAssistant.Services.BackupProgress>(reports.Add);
                await this.BackupServiceInstance.RunIncrementalBackupAsync(@"c:\Source", @"c:\Destination", [], progress, CancellationToken.None);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(original);
            }

            BackupAssistant.Services.BackupProgress lastProgressReport = reports.Last(r => r.Progress.HasValue);
            Assert.Equal(100, lastProgressReport.Progress);
            Assert.Equal("Backup is complete.", reports[^1].Status);
        }

        #endregion
    }
}
