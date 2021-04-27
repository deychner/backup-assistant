using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Tests
{
    [TestClass]
    public class FilterSelection
    {
        private Mock<IBackupStarter> _mock;
        private MockFileSystem _mockFileSystem;

        [TestInitialize]
        public void Initialize()
        {
            _mockFileSystem = new MockFileSystem();
            _mockFileSystem.AddDirectory(@"c:\Source");
            _mockFileSystem.AddDirectory(@"c:\Source\dir1");
            _mockFileSystem.AddDirectory(@"c:\Source\dir1\dir11");
            _mockFileSystem.AddDirectory(@"c:\Source\dir2");

            _mock = new Mock<IBackupStarter>(MockBehavior.Strict);
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Source");
        }

        [TestMethod]
        public void Initialize_NoFilters()
        {
            _mock.Setup(f => f.Filters).Returns(new List<string>(new string[] { }));
            var form = new Modals.FilterSelection(_mock.Object, _mockFileSystem);

            // Check filter presence
            Assert.AreEqual(2, form.FilterSelection_CheckedListBox_Filters.Items.Count);
            Assert.IsTrue(form.FilterSelection_CheckedListBox_Filters.Items.Contains(@"...\dir1"));
            Assert.IsTrue(form.FilterSelection_CheckedListBox_Filters.Items.Contains(@"...\dir2"));
        }

        [TestMethod]
        public void Initialize_CheckState_NoFilters()
        {
            _mock.Setup(f => f.Filters).Returns(new List<string>(new string[] { }));
            var form = new Modals.FilterSelection(_mock.Object, _mockFileSystem);

            // Validate check state
            Assert.AreEqual(0, form.FilterSelection_CheckedListBox_Filters.CheckedItems.Count);
        }

        [TestMethod]
        public void Initialize_CheckState_Filters()
        {
            _mock.Setup(f => f.Filters).Returns(new List<string>(new string[] { @"...\dir1" }));
            var form = new Modals.FilterSelection(_mock.Object, _mockFileSystem);

            // Validate check state
            Assert.AreEqual(1, form.FilterSelection_CheckedListBox_Filters.CheckedItems.Count);
            Assert.IsTrue(form.FilterSelection_CheckedListBox_Filters.CheckedItems.Contains(@"...\dir1"));
        }

        [TestMethod]
        public void GetFilterList()
        {
            _mock.Setup(f => f.Filters).Returns(new List<string>(new string[] { @"...\dir1" }));
            var form = new Modals.FilterSelection(_mock.Object, _mockFileSystem);

            var items = form.GetFilterList();

            // Validate
            Assert.AreEqual(1, items.Count);
            Assert.IsTrue(items.Contains(@"...\dir1"));
        }
    }
}
