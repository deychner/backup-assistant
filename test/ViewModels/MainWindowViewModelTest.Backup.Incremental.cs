using BackupAssistant.DataModels;
using BackupAssistant.Test.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackupIncremental : MainWindowViewModelTestBase
    {
        #region GetCombinedFileList

        [Fact]
        public async Task GetCombinedFileList_SingleLevel_SourceOnly()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file.txt");
            this.FileSystemMock.Directory.CreateDirectory(@"c:\Destination");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            Assert.Single(fileList);
            Assert.Contains(fileList, (f) => f.Key == @"...\file.txt");
            Assert.True(fileList[@"...\file.txt"].IsInSource);
            Assert.False(fileList[@"...\file.txt"].IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_SingleLevel_DestinationOnly()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file.txt");
            this.FileSystemMock.Directory.CreateDirectory(@"c:\Source");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            Assert.Single(fileList);
            Assert.Contains(fileList, (f) => f.Key == @"...\file.txt");
            Assert.False(fileList[@"...\file.txt"].IsInSource);
            Assert.True(fileList[@"...\file.txt"].IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_SingleLevel_Both()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file.txt");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            Assert.Single(fileList);
            Assert.Contains(fileList, (f) => f.Key == @"...\file.txt");
            Assert.True(fileList[@"...\file.txt"].IsInSource);
            Assert.True(fileList[@"...\file.txt"].IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_MultiLevel_SourceOnly()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\SubDirectory\file.txt");
            this.FileSystemMock.Directory.CreateDirectory(@"c:\Destination");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            Assert.Single(fileList);
            Assert.Contains(fileList, (f) => f.Key == @"...\SubDirectory\file.txt");
            Assert.True(fileList[@"...\SubDirectory\file.txt"].IsInSource);
            Assert.False(fileList[@"...\SubDirectory\file.txt"].IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_MultiLevel_DestinationOnly()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\SubDirectory\file.txt");
            this.FileSystemMock.Directory.CreateDirectory(@"c:\Source");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            Assert.Single(fileList);
            Assert.Contains(fileList, (f) => f.Key == @"...\SubDirectory\file.txt");
            Assert.False(fileList[@"...\SubDirectory\file.txt"].IsInSource);
            Assert.True(fileList[@"...\SubDirectory\file.txt"].IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_MultiLevel_Both()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\SubDirectory\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\SubDirectory\file.txt");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync();

            Assert.Single(fileList);
            Assert.Contains(fileList, (f) => f.Key == @"...\SubDirectory\file.txt");
            Assert.True(fileList[@"...\SubDirectory\file.txt"].IsInSource);
            Assert.True(fileList[@"...\SubDirectory\file.txt"].IsInDestination);
        }

        [Fact]
        public async Task GetCombinedFileList_Filters()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\Search\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\Ignore\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\Search\file.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\Ignore\file.txt");

            IDictionary<string, FileListing> fileList = await GetCombinedFileListAsync([@"...\Search"]);

            Assert.Equal(2, fileList.Count);

            Assert.Contains(fileList, (f) => f.Key == @"...\file.txt");
            Assert.True(fileList[@"...\file.txt"].IsInSource);
            Assert.True(fileList[@"...\file.txt"].IsInDestination);

            Assert.Contains(fileList, (f) => f.Key == @"...\Search\file.txt");
            Assert.True(fileList[@"...\Search\file.txt"].IsInSource);
            Assert.True(fileList[@"...\Search\file.txt"].IsInDestination);
        }

        private async Task<IDictionary<string, FileListing>> GetCombinedFileListAsync()
        {
            return await GetCombinedFileListAsync([]);
        }

        private async Task<IDictionary<string, FileListing>> GetCombinedFileListAsync(ObservableCollection<string> filters)
        {
            this.ViewModelInstance!.Model.Source = @"c:\Source";
            this.ViewModelInstance.Model.Destination = @"c:\Destination";
            this.ViewModelInstance.Model.Filters = filters;

            return await this.ViewModelInstance.GetCombinedFileListAsync(@"c:\Source", @"c:\Destination", CancellationToken.None);
        }

        #endregion

        #region RunIncrementalBackupInternal

        [Fact]
        public async Task RunIncrementalBackupInternal_NoAction()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            this.FileSystemMock.AddFile(@"c:\Source\file.txt", new MockFileData("Sample data") { LastWriteTime = now });
            this.FileSystemMock.AddFile(@"c:\Destination\file.txt", new MockFileData("Sample data") { LastWriteTime = now });

            await RunIncrementalBackup();

            // Check that the file was not touched
            Assert.True(now == this.FileSystemMock.FileInfo.New(@"c:\Destination\file.txt").LastWriteTime, "The file was updated.");
        }

        [Fact]
        public async Task RunIncrementalBackupInternal_Copy()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file.txt");
            this.FileSystemMock.Directory.CreateDirectory(@"c:\Destination");

            await RunIncrementalBackup();

            // Check that the file was copied
            Assert.True(this.FileSystemMock.FileExists(@"c:\Destination\file.txt"), "The file was not copied.");
        }

        [Fact]
        public async Task RunIncrementalBackupInternal_Overwrite()
        {
            DateTimeOffset now = DateTimeOffset.Now;
            this.FileSystemMock.AddFile(@"c:\Source\file.txt", new MockFileData("Sample data") { LastWriteTime = now });
            this.FileSystemMock.AddFile(@"c:\Destination\file.txt", new MockFileData("Sample data") { LastWriteTime = now.AddMinutes(-1) });

            await RunIncrementalBackup();

            // Check that the file was touched
            Assert.True(now == this.FileSystemMock.FileInfo.New(@"c:\Destination\file.txt").LastWriteTime, "The file was not updated.");
        }

        [Fact]
        public async Task RunIncrementalBackupInternal_Delete()
        {
            this.FileSystemMock.Directory.CreateDirectory(@"c:\Source");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file.txt");

            await RunIncrementalBackup();

            // Check that the file was deleted
            Assert.False(this.FileSystemMock.FileExists(@"c:\Destination\file.txt"), "The file was not deleted.");
        }

        private async Task RunIncrementalBackup()
        {
            this.ViewModelInstance!.Model.Source = @"c:\Source";
            this.ViewModelInstance.Model.Destination = @"c:\Destination";
            this.ViewModelInstance.Model.Filters = [];

            await this.ViewModelInstance.RunIncrementalBackupInternalAsync(CancellationToken.None);
        }

        #endregion
    }
}
