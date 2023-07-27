using BackupAssistant.DataModels;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackupIncremental : MainWindowViewModelTestBase
    {
        [Fact]
        public void GetCombinedFileList_SingleLevel_SourceOnly()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Single", @"c:\Single_Backup", CancellationToken.None);

            Assert.Equal(3, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\file1.txt");
            Assert.True(fileList[@"...\file1.txt"].IsInSource);
            Assert.False(fileList[@"...\file1.txt"].IsInDestination);
        }

        [Fact]
        public void GetCombinedFileList_SingleLevel_Overlap()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Single", @"c:\Single_Backup", CancellationToken.None);

            Assert.Equal(3, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\file2.txt");
            Assert.True(fileList[@"...\file2.txt"].IsInSource);
            Assert.True(fileList[@"...\file2.txt"].IsInDestination);
        }

        [Fact]
        public void GetCombinedFileList_SingleLevel_DestinationOnly()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Single", @"c:\Single_Backup", CancellationToken.None);

            Assert.Equal(3, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\file3.txt");
            Assert.False(fileList[@"...\file3.txt"].IsInSource);
            Assert.True(fileList[@"...\file3.txt"].IsInDestination);
        }

        [Fact]
        public void GetCombinedFileList_MultiLevel_SourceOnly()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup", CancellationToken.None);

            Assert.Equal(5, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\L1F1\file2.txt");
            Assert.True(fileList[@"...\L1F1\file2.txt"].IsInSource);
            Assert.False(fileList[@"...\L1F1\file2.txt"].IsInDestination);
        }

        [Fact]
        public void GetCombinedFileList_MultiLevel_Overlap()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup", CancellationToken.None);

            Assert.Equal(5, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\L1F2\file3.txt");
            Assert.True(fileList[@"...\L1F2\file3.txt"].IsInSource);
            Assert.True(fileList[@"...\L1F2\file3.txt"].IsInDestination);
        }

        [Fact]
        public void GetCombinedFileList_MultiLevel_DestinationOnly()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup", CancellationToken.None);

            Assert.Equal(5, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\L1F1\file5.txt");
            Assert.False(fileList[@"...\L1F1\file5.txt"].IsInSource);
            Assert.True(fileList[@"...\L1F1\file5.txt"].IsInDestination);
        }

        [Fact]
        public void GetCombinedFileList_MultiLevel_Filters()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>() { @"...\L1F1" };

            IDictionary<string, FileListing> fileList = this.ViewModelInstance.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup", CancellationToken.None);

            Assert.Equal(4, fileList.Count);
            Assert.Contains(fileList, (f) => f.Key == @"...\file1.txt");
            Assert.Contains(fileList, (f) => f.Key == @"...\L1F1\file2.txt");
            Assert.Contains(fileList, (f) => f.Key == @"...\L1F1\L2F1\file4.txt");
            Assert.Contains(fileList, (f) => f.Key == @"...\L1F1\file5.txt");
        }

        [Fact]
        public void RunIncrementalBackupInternal_SingleLevel_NoAction()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Single_Backup\file2.txt"));
        }

        [Fact]
        public void RunIncrementalBackupInternal_SingleLevel_Copy()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Single_Backup\file1.txt"));
        }

        [Fact]
        public void RunIncrementalBackupInternal_SingleLevel_Delete()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            Assert.False(this.FileSystemMock.File.Exists(@"c:\Single_Backup\file3.txt"));
        }

        [Fact]
        public void RunIncrementalBackupInternal_SingleLevel_Overwrite()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Single_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            // Check dates
            DateTime sourceLastModified = this.FileSystemMock.FileInfo.New(@"c:\Single\file2.txt").LastWriteTime;
            DateTime destinationLastModified = this.FileSystemMock.FileInfo.New(@"c:\Single_Backup\file2.txt").LastWriteTime;

            if (sourceLastModified <= destinationLastModified)
            {
                Assert.Fail("The test did not make a source file that is newer than the destination file.");
            }

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            // Refresh dates
            sourceLastModified = this.FileSystemMock.FileInfo.New(@"c:\Single\file2.txt").LastWriteTime;
            destinationLastModified = this.FileSystemMock.FileInfo.New(@"c:\Single_Backup\file2.txt").LastWriteTime;

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Single_Backup\file2.txt"));
            Assert.True(destinationLastModified >= sourceLastModified);
        }

        [Fact]
        public void RunIncrementalBackupInternal_MultiLevel_NoAction()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Multi_Backup\L1F2\file3.txt"));
        }

        [Fact]
        public void RunIncrementalBackupInternal_MultiLevel_Copy()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Multi_Backup\L1F1\file2.txt"), "File 2 not found.");
        }

        [Fact]
        public void RunIncrementalBackupInternal_MultiLevel_Delete()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            Assert.False(this.FileSystemMock.File.Exists(@"c:\Multi_Backup\L1F1\file5.txt"), "File 5 was found.");
        }

        [Fact]
        public void RunIncrementalBackupInternal_MultiLevel_Overwrite()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Multi_Backup";
            this.ViewModelInstance.Model.Filters = new ObservableCollection<string>();

            // Touch file3.txt in source to make it more recent
            this.FileSystemMock.File.WriteAllText(@"c:\Multi\L1F2\file3.txt", "New content");

            // Check dates
            DateTime sourceLastModified = this.FileSystemMock.FileInfo.New(@"c:\Multi\L1F2\file3.txt").LastWriteTime;
            DateTime destinationLastModified = this.FileSystemMock.FileInfo.New(@"c:\Multi_Backup\L1F2\file3.txt").LastWriteTime;

            if (sourceLastModified < destinationLastModified)
            {
                Assert.Fail("The test did not make a source file that is newer than the destination file.");
            }

            this.ViewModelInstance.RunIncrementalBackupInternal(CancellationToken.None);

            // Refresh dates
            sourceLastModified = this.FileSystemMock.FileInfo.New(@"c:\Multi\L1F2\file3.txt").LastWriteTime;
            destinationLastModified = this.FileSystemMock.FileInfo.New(@"c:\Multi_Backup\L1F2\file3.txt").LastWriteTime;

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Multi_Backup\L1F2\file3.txt"), "File 3 not found.");
            Assert.True(destinationLastModified >= sourceLastModified, "File 3 was not updated.");
        }

        private void CreateMockFiles()
        {
            MockFileData mockFileData = new("Sample data");

            // Add source files
            this.FileSystemMock.AddFile(@"c:\Single\file1.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Single\file2.txt", new MockFileData("Sample data") { LastWriteTime = DateTimeOffset.Now });
            this.FileSystemMock.AddFile(@"c:\Multi\file1.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\L1F1\file2.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\L1F2\file3.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\L1F1\L2F1\file4.txt", mockFileData);

            // Add destination files
            this.FileSystemMock.AddFile(@"c:\Single_Backup\file2.txt", new MockFileData("Sample data") { LastWriteTime = DateTimeOffset.Now.AddDays(-1) });
            this.FileSystemMock.AddFile(@"c:\Single_Backup\file3.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi_Backup\file1.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi_Backup\L1F2\file3.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi_Backup\L1F1\L2F1\file4.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi_Backup\L1F1\file5.txt", mockFileData);
        }
    }
}
