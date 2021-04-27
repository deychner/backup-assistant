using BackupAssistant.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

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
            IFileInfo fileInfo = null;

            try
            {
                fileInfo = _backupAgent.SafeGetFileInfo(null);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.IsNull(fileInfo, "The file info was not null.");
        }

        [TestMethod]
        public void SafeGetFiles()
        {
            IReadOnlyCollection<string> files = null;

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
            IReadOnlyCollection<string> directories = null;

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
        public void EnsureDirectoryPathExists()
        {
            MockFileSystem fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(@"c:\");

            _backupAgent = new Core.BackupAgent(_mock.Object, fileSystem);

            try
            {
                _backupAgent.EnsureDirectoryPathExists(@"c:\level1\level2\file.txt");
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }

            Assert.IsTrue(fileSystem.Directory.Exists(@"c:\level1"), "Top level directory was not created.");
            Assert.IsTrue(fileSystem.Directory.Exists(@"c:\level1\level2"), "Top level directory was not created.");
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
                _backupAgent.SafeDeleteFile(null);
            }
            catch (Exception e)
            {
                Assert.Fail($"Expected no exception to be thrown. Message: {e.Message}");
            }
        }
    }
}
