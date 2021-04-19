using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

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

            _backupAgent = new Core.BackupAgent(_mock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CallerNull()
        {
            _backupAgent = new Core.BackupAgent(null);
            _backupAgent.ValidateBackup();
        }

        [TestMethod]
        public void SourcePathNull()
        {
            _mock.Setup(s => s.SourcePath).Returns(string.Empty);

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("You must specify a backup source."));
        }

        [TestMethod]
        public void SourcePathInvalid()
        {
            _mock.Setup(s => s.SourcePath).Returns(Guid.NewGuid().ToString());

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("The specified source directory could not be found."));
        }

        [TestMethod]
        public void DestinationPathNull()
        {
            _mock.Setup(s => s.DestinationPath).Returns(string.Empty);

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("You must specify a backup destination."));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DestinationPathInvalid()
        {
            _mock.Setup(s => s.DestinationPath).Returns(string.Empty);

            var e = Assert.ThrowsException<ArgumentException>(() => _backupAgent.ValidateBackup());
            Assert.IsTrue(e.Message.Contains("The specified destination directory could not be found."));
        }
    }
}
