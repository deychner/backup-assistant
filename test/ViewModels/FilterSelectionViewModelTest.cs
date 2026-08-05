using BackupAssistant.DataModels;
using BackupAssistant.Test.ViewModels.Base;
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
            this.InMemoryFileSystem.AddDirectory(@"c:\Source");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir1");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir1\dir3");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir2");

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
            _ = Assert.Single(actual);
            Assert.Equal("test1", actual[0]);
        }

        [Fact]
        public void PopulateFilterList_NoFilters()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\Source");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir1");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir1\dir3");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir2");

            this.ViewModelInstance.Model.RootPath = @"c:\Source";

            this.ViewModelInstance.PopulateFilterList([]);

            Assert.Equal(2, this.ViewModelInstance.Model.FilterItems.Count);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && !f.IsChecked);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2") && !f.IsChecked);
            Assert.DoesNotContain(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir3"));
        }

        [Fact]
        public void PopulateFilterList_Filters()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\Source");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir1");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir2");

            this.ViewModelInstance.Model.RootPath = @"c:\Source";

            this.ViewModelInstance.PopulateFilterList([@"...\dir1"]);

            Assert.Equal(2, this.ViewModelInstance.Model.FilterItems.Count);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && f.IsChecked);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2") && !f.IsChecked);
        }

        [Fact]
        public void PopulateFilterList_UnreadableRootYieldsNothing()
        {
            // The source folder was renamed or unplugged since it was last chosen
            this.ViewModelInstance.Model.RootPath = @"c:\gone";

            this.ViewModelInstance.PopulateFilterList([]);

            Assert.Empty(this.ViewModelInstance.Model.FilterItems);
        }

        [Fact]
        public void FilterItems_CanBeReplaced()
        {
            ObservableCollection<FilterItem> replacement = [new FilterItem { Path = "test", IsChecked = true }];

            this.ViewModelInstance.FilterItems = replacement;

            Assert.Same(replacement, this.ViewModelInstance.FilterItems);
            Assert.Same(replacement, this.ViewModelInstance.Model.FilterItems);
        }

        [Fact]
        public void PopulateFilterList_IgnoreHidden()
        {
            this.InMemoryFileSystem.AddDirectory(@"c:\Source");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir1");
            this.InMemoryFileSystem.AddDirectory(@"c:\Source\dir2");
            this.InMemoryFileSystem.DirectoryInfo.New(@"c:\Source\dir2").Attributes = FileAttributes.Hidden;

            this.ViewModelInstance.Model.RootPath = @"c:\Source";

            this.ViewModelInstance.PopulateFilterList([]);

            _ = Assert.Single(this.ViewModelInstance.Model.FilterItems);
            Assert.Contains(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir1") && !f.IsChecked);
            Assert.DoesNotContain(this.ViewModelInstance.Model.FilterItems, (f) => f.Path.Equals(@"...\dir2"));
        }

        private class FilterSelectionImposter
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
