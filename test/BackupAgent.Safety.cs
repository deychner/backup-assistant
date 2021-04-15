using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace BackupAssistant.Tests.BackupAgent
{
    [TestClass]
    public class Safety
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

        [TestMethod]
        public void SafeGetFileInfo()
        {
            FileInfo fileInfo = null;

            try
            {
                fileInfo = _backupAgent.SafeGetFileInfo(Guid.NewGuid().ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.IsNotNull(fileInfo, "The file info was null.");
        }

        [TestMethod]
        public void SafeGetFiles()
        {
            ReadOnlyCollection<string> files = null;

            try
            {
                files = _backupAgent.SafeGetFiles(Guid.NewGuid().ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.IsNotNull(files, "The file list was null.");
            Assert.AreEqual(0, files.Count, "Expected an empty set.");
        }

        [TestMethod]
        public void SafeGetDirectories()
        {
            ReadOnlyCollection<string> directories = null;

            try
            {
                directories = _backupAgent.SafeGetDirectories(Guid.NewGuid().ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.IsNotNull(directories, "The directory list was null.");
            Assert.AreEqual(0, directories.Count, "Expected an empty set.");
        }

        [TestMethod]
        public void SafeCopyFile()
        {
            try
            {
                _backupAgent.SafeCopyFile(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }


        [TestMethod]
        public void SafeDeleteFile()
        {
            try
            {
                _backupAgent.SafeDeleteFile(Guid.NewGuid().ToString());
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }
    }
}
