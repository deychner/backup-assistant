using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BackupAssistant.Tests.FileListing
{
    [TestClass]
    public class BackupAction
    {
        private Core.FileListing _fileListing;

        [TestInitialize]
        public void Initialize()
        {
            _fileListing = new Core.FileListing();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _fileListing = null;
        }

        [TestMethod]
        public void NotSource_NotDestination()
        {
            _fileListing.IsInSource = false;
            _fileListing.IsInDestination = false;
            _fileListing.SourceLastModified = DateTime.Today;
            _fileListing.DestinationLastModified = DateTime.Today;

            Assert.AreEqual(Core.BackupAction.None, _fileListing.GetBackupAction(), "Incorrect backup action determined.");
        }

        [TestMethod]
        public void Source_NotDestination()
        {
            _fileListing.IsInSource = true;
            _fileListing.IsInDestination = false;
            _fileListing.SourceLastModified = DateTime.Today;
            _fileListing.DestinationLastModified = DateTime.Today;

            Assert.AreEqual(Core.BackupAction.Copy, _fileListing.GetBackupAction(), "Incorrect backup action determined.");
        }

        [TestMethod]
        public void NotSource_Destination()
        {
            _fileListing.IsInSource = false;
            _fileListing.IsInDestination = true;
            _fileListing.SourceLastModified = DateTime.Today;
            _fileListing.DestinationLastModified = DateTime.Today;

            Assert.AreEqual(Core.BackupAction.Delete, _fileListing.GetBackupAction(), "Incorrect backup action determined.");
        }

        [TestMethod]
        public void Source_Destination_SourceNewer()
        {
            _fileListing.IsInSource = true;
            _fileListing.IsInDestination = true;
            _fileListing.SourceLastModified = DateTime.Today;
            _fileListing.DestinationLastModified = DateTime.Today.AddDays(-1);

            Assert.AreEqual(Core.BackupAction.Overwrite, _fileListing.GetBackupAction(), "Incorrect backup action determined.");
        }

        [TestMethod]
        public void Source_Destination_DestinationNewer()
        {
            _fileListing.IsInSource = true;
            _fileListing.IsInDestination = true;
            _fileListing.SourceLastModified = DateTime.Today.AddDays(-1);
            _fileListing.DestinationLastModified = DateTime.Today;

            Assert.AreEqual(Core.BackupAction.None, _fileListing.GetBackupAction(), "Incorrect backup action determined.");
        }
    }
}
