using BackupAssistant.Test.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels
{
    public class MainWindowViewModelTestBackupFull : MainWindowViewModelTestBase
    {
        #region GetFileList

        [Fact]
        public void GetFileList_SingleLevel()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file2.txt");

            IList<string> fileList = GetFileList();

            Assert.Equal(2, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\file2.txt", fileList);
        }

        [Fact]
        public void GetFileList_MultiLevel()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\file2.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\L2F1\file4.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F2\file3.txt");

            IList<string> fileList = GetFileList();

            Assert.Equal(4, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\file2.txt", fileList);
            Assert.Contains(@"c:\Source\L1F2\file3.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\L2F1\file4.txt", fileList);
        }

        [Fact]
        public void GetFileList_MultiLevel_Filters()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\file2.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F1\L2F1\file4.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\L1F2\file3.txt");

            IList<string> fileList = GetFileList([@"...\L1F1"]);

            Assert.Equal(3, fileList.Count);
            Assert.Contains(@"c:\Source\file1.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\file2.txt", fileList);
            Assert.Contains(@"c:\Source\L1F1\L2F1\file4.txt", fileList);
        }

        private IList<string> GetFileList()
        {
            return GetFileList([]);
        }

        private IList<string> GetFileList(ObservableCollection<string> filters)
        {
            this.ViewModelInstance!.Model.Filters = filters;

            return this.ViewModelInstance!.GetFileList(@"c:\Source", CancellationToken.None);
        }

        #endregion

        #region RunFullBackupInternal

        [Fact]
        public void RunFullBackupInternal_Basic()
        {
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file1.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Source\file2.txt");
            this.FileSystemMock.AddEmptyFile(@"c:\Destination\file3.txt");

            RunFullBackupInternal();

            Assert.True(this.FileSystemMock.File.Exists(@"c:\Destination\file1.txt"));
            Assert.True(this.FileSystemMock.File.Exists(@"c:\Destination\file2.txt"));
            Assert.False(this.FileSystemMock.File.Exists(@"c:\Destination\file3.txt"));
        }

        private void RunFullBackupInternal()
        {
            this.ViewModelInstance!.Model.Source = @"c:\Source";
            this.ViewModelInstance.Model.Destination = @"c:\Destination";
            this.ViewModelInstance.Model.Filters = [];

            this.ViewModelInstance.RunFullBackupInternal(CancellationToken.None);
        }

        #endregion
    }
}
