Imports BackupAssistant.Core
Imports System.Collections.ObjectModel
Imports System.IO

<TestClass()>
Public Class BackupAgent_Safety
    Implements IBackupStarter

    Private _backupAgent As BackupAgent

#Region "Interface methods"

    Friend Sub PreProcess() Implements IBackupStarter.PreProcess
        ' Do nothing. This does not apply to a test.
    End Sub

    Friend Sub PostProcess() Implements IBackupStarter.PostProcess
        ' Do nothing. This does not apply to a test.
    End Sub

    Private _destinationPath As String
    Friend ReadOnly Property DestinationPath As String Implements IBackupStarter.DestinationPath
        Get
            Return _destinationPath
        End Get
    End Property

    Friend Sub ReportProgress(progress As Integer) Implements IBackupStarter.ReportProgress
        ' Do nothing. This does not apply to a test.
    End Sub

    Friend Sub ReportStatus(status As String) Implements IBackupStarter.ReportStatus
        ' Do nothing. This does not apply to a test.
    End Sub

    Friend Sub AddToLogEntry(message As String) Implements IBackupStarter.AddToLogEntry
        ' Do nothing. This does not apply to a test.
    End Sub

    Private _sourcePath As String
    Friend ReadOnly Property SourcePath As String Implements IBackupStarter.SourcePath
        Get
            Return _sourcePath
        End Get
    End Property

    Private _filterList As List(Of String)
    Private ReadOnly Property Filters As ReadOnlyCollection(Of String) Implements IBackupStarter.Filters
        Get
            Return _filterList.AsReadOnly
        End Get
    End Property

#End Region

    <TestInitialize()>
    Public Sub TestInitialize()
        _backupAgent = New BackupAgent(Me)
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Failure_SafeGetFileInfo()
        Dim fileInfo As FileInfo = Nothing

        Try
            fileInfo = _backupAgent.SafeGetFileInfo(Guid.NewGuid.ToString)
        Catch ex As Exception
            Assert.Fail("Expected no exception to be thrown. Message: " & ex.Message)
        End Try

        Assert.IsNotNull(fileInfo, "The file info was null.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Failure_SafeGetFiles()
        Dim files As ReadOnlyCollection(Of String) = Nothing

        Try
            files = _backupAgent.SafeGetFiles(Guid.NewGuid.ToString)
        Catch ex As Exception
            Assert.Fail("Expected no exception to be thrown. Message: " & ex.Message)
        End Try

        Assert.IsNotNull(files, "The file list was null.")
        Assert.AreEqual(0, files.Count, "Expected an empty set.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Failure_SafeGetDirectories()
        Dim directories As ReadOnlyCollection(Of String) = Nothing

        Try
            directories = _backupAgent.SafeGetDirectories(Guid.NewGuid.ToString)
        Catch ex As Exception
            Assert.Fail("Expected no exception to be thrown. Message: " & ex.Message)
        End Try

        Assert.IsNotNull(directories, "The directory list was null.")
        Assert.AreEqual(0, directories.Count, "Expected an empty set.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Failure_SafeCopyFile()
        Try
            _backupAgent.SafeCopyFile(Guid.NewGuid.ToString, Guid.NewGuid.ToString, True)
        Catch ex As Exception
            Assert.Fail("Expected no exception to be thrown. Message: " & ex.Message)
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Failure_SafeDeleteFile()
        Try
            _backupAgent.SafeDeleteFile(Guid.NewGuid.ToString)
        Catch ex As Exception
            Assert.Fail("Expected no exception to be thrown. Message: " & ex.Message)
        End Try
    End Sub

End Class