using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO.Abstractions.TestingHelpers;

namespace BackupAssistant.Tests.BackupAgent
{
    [TestClass]
    public class Validation
    {
        private Mock<IBackupStarter> _mock;
        private Core.BackupAgent _backupAgent;

        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IBackupStarter>(MockBehavior.Strict);

            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(@"C:\test");

            _backupAgent = new Core.BackupAgent(_mock.Object, fileSystem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallerNull()
        {
            _backupAgent = new Core.BackupAgent(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileSystemNull()
        {
            _backupAgent = new Core.BackupAgent(_mock.Object, null);
        }

        [TestMethod]
        public void SourcePathNull()
        {
            _mock.Setup(s => s.SourcePath).Returns(string.Empty);
            _mock.Setup(s => s.DestinationPath).Returns(@"C:\test");

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("You must specify a backup source."));
        }

        [TestMethod]
        public void SourcePathInvalid()
        {
            _mock.Setup(s => s.SourcePath).Returns(Guid.NewGuid().ToString());
            _mock.Setup(s => s.DestinationPath).Returns(@"C:\test");

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("The specified source directory could not be found."));
        }

        [TestMethod]
        public void DestinationPathNull()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"C:\test");
            _mock.Setup(s => s.DestinationPath).Returns(string.Empty);

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("You must specify a backup destination."));
        }

        [TestMethod]
        public void DestinationPathInvalid()
        {
            _mock.Setup(s => s.SourcePath).Returns(@"C:\test");
            _mock.Setup(s => s.DestinationPath).Returns(Guid.NewGuid().ToString());

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("The specified destination directory could not be found."));
        }
    }
}
