using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            _mock = new Mock<IBackupStarter>(MockBehavior.Strict);
            _mock.Setup(a => a.AddToLogEntry(It.IsAny<string>()));

            _backupAgent = new Core.BackupAgent(_mock.Object);
        }
    }
}
