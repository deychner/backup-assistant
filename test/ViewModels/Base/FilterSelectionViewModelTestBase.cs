using BackupAssistant.ViewModels;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels.Base
{
    public class FilterSelectionViewModelTestBase
    {
        protected readonly MockFileSystem FileSystemMock;

        protected readonly FilterSelectionViewModel ViewModelInstance;

        public FilterSelectionViewModelTestBase()
        {
            FileSystemMock = new MockFileSystem();

            ViewModelInstance = new FilterSelectionViewModel(FileSystemMock);
        }
    }
}
