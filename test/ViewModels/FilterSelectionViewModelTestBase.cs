using BackupAssistant.ViewModels;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels
{
    public class FilterSelectionViewModelTestBase
    {
        protected readonly MockFileSystem FileSystemMock;

        protected readonly FilterSelectionViewModel ViewModelInstance;

        public FilterSelectionViewModelTestBase()
        {
            this.FileSystemMock = new MockFileSystem();

            this.ViewModelInstance = new FilterSelectionViewModel(FileSystemMock);
        }
    }
}
