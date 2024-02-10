using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackupFull : MainWindowViewModelTestBase
    {
        [Fact]
        public void GetFileList_SingleLevel()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Filters = [];

            IList<string> fileList = this.ViewModelInstance!.GetFileList(@"c:\Single", CancellationToken.None);

            Assert.Equal(2, fileList.Count);
            Assert.Contains(@"c:\Single\file1.txt", fileList);
            Assert.Contains(@"c:\Single\file2.txt", fileList);
        }

        [Fact]
        public void GetFileList_MultiLevel()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Filters = [];

            IList<string> fileList = this.ViewModelInstance!.GetFileList(@"c:\Multi", CancellationToken.None);

            Assert.Equal(4, fileList.Count);
            Assert.Contains(@"c:\Multi\file1.txt", fileList);
            Assert.Contains(@"c:\Multi\L1F1\file2.txt", fileList);
            Assert.Contains(@"c:\Multi\L1F2\file3.txt", fileList);
            Assert.Contains(@"c:\Multi\L1F1\L2F1\file4.txt", fileList);
        }

        [Fact]
        public void GetFileList_MultiLevel_Filters()
        {
            CreateMockFiles();

            this.ViewModelInstance!.Model.Filters = [@"...\L1F1"];

            IList<string> fileList = this.ViewModelInstance.GetFileList(@"c:\Multi", CancellationToken.None);

            Assert.Equal(3, fileList.Count);
            Assert.Contains(@"c:\Multi\file1.txt", fileList);
            Assert.Contains(@"c:\Multi\L1F1\file2.txt", fileList);
            Assert.Contains(@"c:\Multi\L1F1\L2F1\file4.txt", fileList);
        }

        [Fact]
        public void RunFullBackupInternal_SingleLevel()
        {
            CreateMockFiles();
            this.FileSystemMock.AddFile(@"c:\Backup\file3.txt", new MockFileData("Sample data"));

            this.ViewModelInstance!.Model.Source = @"c:\Single";
            this.ViewModelInstance.Model.Destination = @"c:\Backup";
            this.ViewModelInstance.Model.Filters = [];

            this.ViewModelInstance.RunFullBackupInternal(CancellationToken.None);

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Backup\file1.txt"));
            Assert.True(this.FileSystemMock.File.Exists(@"c:\Backup\file2.txt"));
            Assert.False(this.FileSystemMock.File.Exists(@"c:\Backup\file3.txt"));
        }

        [Fact]
        public void RunFullBackupInternal_MultiLevel()
        {
            CreateMockFiles();
            this.FileSystemMock.AddFile(@"c:\Backup\L1F1\file5.txt", new MockFileData("Sample data"));

            this.ViewModelInstance!.Model.Source = @"c:\Multi";
            this.ViewModelInstance.Model.Destination = @"c:\Backup";
            this.ViewModelInstance.Model.Filters = [];

            this.ViewModelInstance.RunFullBackupInternal(CancellationToken.None);

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Backup\file1.txt"));
            Assert.True(this.FileSystemMock.File.Exists(@"c:\Backup\L1F1\file2.txt"));
            Assert.True(this.FileSystemMock.File.Exists(@"c:\Backup\L1F2\file3.txt"));
            Assert.True(this.FileSystemMock.File.Exists(@"c:\Backup\L1F1\L2F1\file4.txt"));
            Assert.False(this.FileSystemMock.File.Exists(@"c:\Backup\L1F1\file5.txt"));
        }

        private void CreateMockFiles()
        {
            // Create mock source files
            MockFileData mockFileData = new("Sample data");
            this.FileSystemMock.AddFile(@"c:\Single\file1.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Single\file2.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\file1.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\L1F1\file2.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\L1F2\file3.txt", mockFileData);
            this.FileSystemMock.AddFile(@"c:\Multi\L1F1\L2F1\file4.txt", mockFileData);

            // Create mock destination directory
            this.FileSystemMock.AddDirectory(@"c:\Backup");
        }
    }
}
