using BackupAssistant.DataModels;
using System.Collections.ObjectModel;

namespace BackupAssistant.Test.ViewModels
{
    public class FilterSelectionViewModelTest : FilterSelectionViewModelTestBase
    {
        [Fact]
        public void Input_NoAction()
        {
            this.ViewModelInstance.Input = new FilterSelectionImposter();
            Assert.Equal(string.Empty, this.ViewModelInstance.Model.RootPath);
            Assert.Empty(this.ViewModelInstance.Model.FilterItems);
        }

        [Fact]
        public void Input()
        {
            this.FileSystemMock.AddDirectory(@"c:\Source");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir1");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir1\dir3");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir2");

            this.ViewModelInstance.Input = new FilterSelectionInput()
            {
                RootPath = @"c:\Source"
            };

            Assert.Equal(2, this.ViewModelInstance.Model.FilterItems.Count);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && !f.IsChecked);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2") && !f.IsChecked);
            Assert.DoesNotContain(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir3"));
        }

        [Fact]
        public void Output()
        {
            this.ViewModelInstance.Model.FilterItems.Add(new FilterItem()
            {
                Path = "test1",
                IsChecked = true
            });
            this.ViewModelInstance.Model.FilterItems.Add(new FilterItem()
            {
                Path = "test2",
                IsChecked = false
            });

            ObservableCollection<string> actual = (ObservableCollection<string>)this.ViewModelInstance.Output;
            Assert.Single(actual);
            Assert.Equal("test1", actual[0]);
        }

        [Fact]
        public void PopulateFilterList_NoFilters()
        {
            this.FileSystemMock.AddDirectory(@"c:\Source");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir1");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir1\dir3");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir2");

            this.ViewModelInstance.Model.RootPath = @"c:\Source";

            this.ViewModelInstance.PopulateFilterList(new List<string>());

            Assert.Equal(2, this.ViewModelInstance.Model.FilterItems.Count);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && !f.IsChecked);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2") && !f.IsChecked);
            Assert.DoesNotContain(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir3"));
        }

        [Fact]
        public void PopulateFilterList_Filters()
        {
            this.FileSystemMock.AddDirectory(@"c:\Source");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir1");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir2");

            this.ViewModelInstance.Model.RootPath = @"c:\Source";

            this.ViewModelInstance.PopulateFilterList(new List<string>() { @"...\dir1" });

            Assert.Equal(2, this.ViewModelInstance.Model.FilterItems.Count);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && f.IsChecked);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2") && !f.IsChecked);
        }

        [Fact]
        public void PopulateFilterList_IgnoreHidden()
        {
            this.FileSystemMock.AddDirectory(@"c:\Source");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir1");
            this.FileSystemMock.AddDirectory(@"c:\Source\dir2");
            this.FileSystemMock.DirectoryInfo.New(@"c:\Source\dir2").Attributes = FileAttributes.Hidden;

            this.ViewModelInstance.Model.RootPath = @"c:\Source";

            this.ViewModelInstance.PopulateFilterList(new List<string>());

            Assert.Single(this.ViewModelInstance.Model.FilterItems);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && !f.IsChecked);
            Assert.DoesNotContain(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2"));
        }

        private class FilterSelectionImposter
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
