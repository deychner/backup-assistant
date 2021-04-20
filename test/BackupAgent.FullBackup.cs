using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Tests.BackupAgent
{
    [TestClass]
    public class FullBackup
    {
        private Mock<IBackupStarter> _mock;
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
            MockFileSystem fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\Single\file1.txt", mockFileData },
                { @"c:\Single\file2.txt", mockFileData },
                { @"c:\Multi\file1.txt", mockFileData },
                { @"c:\Multi\L1F1\file2.txt", mockFileData },
                { @"c:\Multi\L1F2\file3.txt", mockFileData },
                { @"c:\Multi\L1F1\L2F1\file4.txt", mockFileData }
            });

            // Create mock destination directory
            fileSystem.AddDirectory(@"c:\Backup");

            _backupAgent = new Core.BackupAgent(_mock.Object, fileSystem);
        }

        [TestMethod]
        public void FileList_SingleLevel()
        {
            _mock.Setup(f => f.Filters).Returns(new ReadOnlyCollection<string>(new string[] { }));

            List<string> fileList = (List<string>)_backupAgent.GetFileList(@"c:\Single");

            Assert.AreEqual(2, fileList.Count, "Incorrect number of files returned.");
            Assert.IsTrue(fileList.Contains(@"c:\Single\file1.txt"), "File 1 not found.");
            Assert.IsTrue(fileList.Contains(@"c:\Single\file2.txt"), "File 2 not found.");
        }
    }
}
