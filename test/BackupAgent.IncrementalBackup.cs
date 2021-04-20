using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Tests.BackupAgent
{
    [TestClass]
    public class IncrementalBackup
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
                { @"c:\Multi\L1F1\L2F1\file4.txt", mockFileData },

                { @"c:\Single_Backup\file2.txt", mockFileData },
                { @"c:\Single_Backup\file3.txt", mockFileData },
                { @"c:\Multi_Backup\file1.txt", mockFileData },
                { @"c:\Multi_Backup\L1F2\file3.txt", mockFileData },
                { @"c:\Multi_Backup\L1F1\L2F1\file4.txt", mockFileData },
                { @"c:\Multi_Backup\L1F1\file5.txt", mockFileData }
            });

            _backupAgent = new Core.BackupAgent(_mock.Object, fileSystem);
        }

        [TestMethod]
        public void CombinedFileList_SingleLevel_SourceOnly()
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
    }
}
