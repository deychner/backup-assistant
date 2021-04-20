using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Tests.BackupAgent
{
    [TestClass]
    public class IncrementalBackup
    {
        private Mock<IBackupStarter> _mock;
        private MockFileSystem _mockFileSystem;
        private Core.BackupAgent _backupAgent;

        [TestInitialize]
        public void Initialize()
        {
            // Set up basic mock
            _mock = new Mock<IBackupStarter>(MockBehavior.Strict);
            _mock.Setup(p => p.PreProcess());
            _mock.Setup(p => p.PostProcess());
            _mock.Setup(p => p.ReportProgress(It.IsAny<int>()));
            _mock.Setup(p => p.ReportStatus(It.IsAny<string>()));
            _mock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            // Create mock source files
            MockFileData mockFileData = new MockFileData("Sample data");
            _mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\Single\file1.txt", mockFileData },
                { @"c:\Single\file2.txt", mockFileData },
                { @"c:\Multi\file1.txt", mockFileData },
                { @"c:\Multi\L1F1\file2.txt", mockFileData },
                { @"c:\Multi\L1F2\file3.txt", mockFileData },
                { @"c:\Multi\L1F1\L2F1\file4.txt", mockFileData },

                { @"c:\Single_Backup\file2.txt", mockFileData },
                { @"c:\Single_Backup\file3.txt", mockFileData },
                { @"c:\Multi_Backup\file1.txt", mockFileData },
                { @"c:\Multi_Backup\L1F2\file3.txt", mockFileData },
                { @"c:\Multi_Backup\L1F1\L2F1\file4.txt", mockFileData },
                { @"c:\Multi_Backup\L1F1\file5.txt", mockFileData }
            });

            _backupAgent = new Core.BackupAgent(_mock.Object, _mockFileSystem);
        }

        [TestMethod]
        public void GetCombinedFileList_SingleLevel_SourceOnly()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Single", @"c:\Single_Backup");

            Assert.AreEqual(3, fileList.Count, "Incorrect number of files found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\file1.txt"), "File 1 not found.");
            Assert.IsTrue(fileList[@"...\file1.txt"].IsInSource, "File 1 not properly marked as being in the source location.");
            Assert.IsFalse(fileList[@"...\file1.txt"].IsInDestination, "File 1 not properly marked as not being in the destination location.");
        }

        [TestMethod]
        public void GetCombinedFileList_SingleLevel_Overlap()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Single", @"c:\Single_Backup");

            Assert.AreEqual(3, fileList.Count, "Incorrect number of files found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\file2.txt"), "File 2 not found.");
            Assert.IsTrue(fileList[@"...\file2.txt"].IsInSource, "File 2 not properly marked as being in the source location.");
            Assert.IsTrue(fileList[@"...\file2.txt"].IsInDestination, "File 2 not properly marked as being in the destination location.");
        }

        [TestMethod]
        public void GetCombinedFileList_SingleLevel_DestinationOnly()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Single", @"c:\Single_Backup");

            Assert.AreEqual(3, fileList.Count, "Incorrect number of files found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\file3.txt"), "File 3 not found.");
            Assert.IsFalse(fileList[@"...\file3.txt"].IsInSource, "File 3 not properly marked as not being in the source location.");
            Assert.IsTrue(fileList[@"...\file3.txt"].IsInDestination, "File 3 not properly marked as being in the destination location.");
        }

        [TestMethod]
        public void GetCombinedFileList_MultiLevel_SourceOnly()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup");

            Assert.AreEqual(5, fileList.Count, "Incorrect number of files found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\L1F1\file2.txt"), "File 2 not found.");
            Assert.IsTrue(fileList[@"...\L1F1\file2.txt"].IsInSource, "File 2 not properly marked as being in the source location.");
            Assert.IsFalse(fileList[@"...\L1F1\file2.txt"].IsInDestination, "File 2 not properly marked as not being in the destination location.");
        }

        [TestMethod]
        public void GetCombinedFileList_MultiLevel_Overlap()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup");

            Assert.AreEqual(5, fileList.Count, "Incorrect number of files found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\L1F2\file3.txt"), "File 3 not found.");
            Assert.IsTrue(fileList[@"...\L1F2\file3.txt"].IsInSource, "File 3 not properly marked as being in the source location.");
            Assert.IsTrue(fileList[@"...\L1F2\file3.txt"].IsInDestination, "File 3 not properly marked as being in the destination location.");
        }

        [TestMethod]
        public void GetCombinedFileList_MultiLevel_DestinationOnly()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup");

            Assert.AreEqual(5, fileList.Count, "Incorrect number of files found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\L1F1\file5.txt"), "File 5 not found.");
            Assert.IsFalse(fileList[@"...\L1F1\file5.txt"].IsInSource, "File 5 not properly marked as not being in the source location.");
            Assert.IsTrue(fileList[@"...\L1F1\file5.txt"].IsInDestination, "File 5 not properly marked as being in the destination location.");
        }

        [TestMethod]
        public void GetCombinedFileList_MultiLevel_Filters()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { @"...\L1F1" }));

            Dictionary<string, Core.FileListing> fileList = (Dictionary<string, Core.FileListing>)_backupAgent.GetCombinedFileList(@"c:\Multi", @"c:\Multi_Backup");

            Assert.AreEqual(4, fileList.Count, "Incorrect number of files returned.");
            Assert.IsTrue(fileList.ContainsKey(@"...\file1.txt"), "File 1 not found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\L1F1\file2.txt"), "File 2 not found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\L1F1\L2F1\file4.txt"), "File 4 not found.");
            Assert.IsTrue(fileList.ContainsKey(@"...\L1F1\file5.txt"), "File 5 not found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_SingleLevel_NoAction()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            _backupAgent.RunIncrementalBackup();

            Assert.IsTrue(_mockFileSystem.File.Exists(@"c:\Single_Backup\file2.txt"), "File 2 not found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_SingleLevel_Copy()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            _backupAgent.RunIncrementalBackup();

            Assert.IsTrue(_mockFileSystem.File.Exists(@"c:\Single_Backup\file1.txt"), "File 1 not found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_SingleLevel_Delete()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            _backupAgent.RunIncrementalBackup();

            Assert.IsFalse(_mockFileSystem.File.Exists(@"c:\Single_Backup\file3.txt"), "File 3 was found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_SingleLevel_Overwrite()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Single");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Single_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            // Touch file2.txt in source to make it more recent
            _mockFileSystem.File.WriteAllText(@"c:\Single\file2.txt", "New content");

            // Check dates
            DateTime sourceLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Single\file2.txt").LastWriteTime;
            DateTime destinationLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Single_Backup\file2.txt").LastWriteTime;

            if (sourceLastModified < destinationLastModified)
            {
                Assert.Inconclusive("The test did not make a source file that is newer than the destination file.");
            }

            _backupAgent.RunIncrementalBackup();

            // Refresh dates
            sourceLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Single\file2.txt").LastWriteTime;
            destinationLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Single_Backup\file2.txt").LastWriteTime;

            Assert.IsTrue(_mockFileSystem.File.Exists(@"c:\Single_Backup\file2.txt"), "File 2 not found.");
            Assert.IsTrue(destinationLastModified >= sourceLastModified, "File 2 was not updated.");
        }

        [TestMethod]
        public void RunIncrementalBackup_MultiLevel_NoAction()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            _backupAgent.RunIncrementalBackup();

            Assert.IsTrue(_mockFileSystem.File.Exists(@"c:\Multi_Backup\L1F2\file3.txt"), "File 3 not found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_MultiLevel_Copy()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            _backupAgent.RunIncrementalBackup();

            Assert.IsTrue(_mockFileSystem.File.Exists(@"c:\Multi_Backup\L1F1\file2.txt"), "File 2 not found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_MultiLevel_Delete()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            _backupAgent.RunIncrementalBackup();

            Assert.IsFalse(_mockFileSystem.File.Exists(@"c:\Multi_Backup\L1F1\file5.txt"), "File 5 was found.");
        }

        [TestMethod]
        public void RunIncrementalBackup_MultiLevel_Overwrite()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"c:\Multi");
            _mock.Setup(d => d.DestinationPath).Returns(@"c:\Multi_Backup");
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            // Touch file3.txt in source to make it more recent
            _mockFileSystem.File.WriteAllText(@"c:\Multi\L1F2\file3.txt", "New content");

            // Check dates
            DateTime sourceLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Multi\L1F2\file3.txt").LastWriteTime;
            DateTime destinationLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Multi_Backup\L1F2\file3.txt").LastWriteTime;

            if (sourceLastModified < destinationLastModified)
            {
                Assert.Inconclusive("The test did not make a source file that is newer than the destination file.");
            }

            _backupAgent.RunIncrementalBackup();

            // Refresh dates
            sourceLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Multi\L1F2\file3.txt").LastWriteTime;
            destinationLastModified = _mockFileSystem.FileInfo.FromFileName(@"c:\Multi_Backup\L1F2\file3.txt").LastWriteTime;

            Assert.IsTrue(_mockFileSystem.File.Exists(@"c:\Multi_Backup\L1F2\file3.txt"), "File 3 not found.");
            Assert.IsTrue(destinationLastModified >= sourceLastModified, "File 3 was not updated.");
        }
    }
}
