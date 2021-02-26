Imports BackupAssistant.Core

<TestClass()>
Public Class FileListing_BackupAction

    Private _fileListing As FileListing

    <TestInitialize>
    Public Sub Initialize()
        _fileListing = New FileListing
    End Sub

    <TestMethod()>
    Public Sub FileListing_Action_NotSource_NotDestination()
        _fileListing.IsInSource = False
        _fileListing.IsInDestination = False
        _fileListing.SourceLastModified = Date.Today
        _fileListing.DestinationLastModified = Date.Today

        Assert.AreEqual(BackupAction.None, _fileListing.GetBackupAction, "Incorrect backup action determined.")
    End Sub

    <TestMethod()>
    Public Sub FileListing_Action_Source_NotDestination()
        _fileListing.IsInSource = True
        _fileListing.IsInDestination = False
        _fileListing.SourceLastModified = Date.Today
        _fileListing.DestinationLastModified = Date.Today

        Assert.AreEqual(BackupAction.Copy, _fileListing.GetBackupAction, "Incorrect backup action determined.")
    End Sub

    <TestMethod()>
    Public Sub FileListing_Action_NotSource_Destination()
        _fileListing.IsInSource = False
        _fileListing.IsInDestination = True
        _fileListing.SourceLastModified = Date.Today
        _fileListing.DestinationLastModified = Date.Today

        Assert.AreEqual(BackupAction.Delete, _fileListing.GetBackupAction, "Incorrect backup action determined.")
    End Sub

    <TestMethod()>
    Public Sub FileListing_Action_Source_Destination_SourceNewer()
        _fileListing.IsInSource = True
        _fileListing.IsInDestination = True
        _fileListing.SourceLastModified = Date.Today
        _fileListing.DestinationLastModified = Date.Today.AddDays(-1)

        Assert.AreEqual(BackupAction.Overwrite, _fileListing.GetBackupAction, "Incorrect backup action determined.")
    End Sub

    <TestMethod()>
    Public Sub FileListing_Action_Source_Destination_DestinationNewer()
        _fileListing.IsInSource = True
        _fileListing.IsInDestination = True
        _fileListing.SourceLastModified = Date.Today.AddDays(-1)
        _fileListing.DestinationLastModified = Date.Today

        Assert.AreEqual(BackupAction.None, _fileListing.GetBackupAction, "Incorrect backup action determined.")
    End Sub

End Class