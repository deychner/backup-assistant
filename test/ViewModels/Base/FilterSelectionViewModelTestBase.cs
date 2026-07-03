using BackupAssistant.ViewModels;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Test.ViewModels.Base
{
    public class FilterSelectionViewModelTestBase
    {
        protected readonly MockFileSystem InMemoryFileSystem;

        protected readonly FilterSelectionViewModel ViewModelInstance;

        public FilterSelectionViewModelTestBase()
        {
            InMemoryFileSystem = new MockFileSystem();

            ViewModelInstance = new FilterSelectionViewModel(InMemoryFileSystem);
        }
    }
}
