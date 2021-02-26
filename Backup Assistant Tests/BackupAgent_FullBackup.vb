Imports BackupAssistant.Core
Imports System.IO
Imports System.Collections.ObjectModel

<TestClass()>
Public Class BackupAgent_FullBackup
    Implements IBackupStarter

    Private Const SAMPLE_DATA As String = "Sample data"

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
        ' Single
        '   file1.txt
        '   file2.txt
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Single")
        File.WriteAllText(Path.GetTempPath & "Single\file1.txt", SAMPLE_DATA)
        File.WriteAllText(Path.GetTempPath & "Single\file2.txt", SAMPLE_DATA)

        ' Multi
        '   file1.txt
        '   L1F1
        '     file2.txt
        '     L2F1
        '       file4.txt
        '   L1F2
        '     file3.txt
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi")
        File.WriteAllText(Path.GetTempPath & "Multi\file1.txt", SAMPLE_DATA)
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi\L1F1")
        File.WriteAllText(Path.GetTempPath & "Multi\L1F1\file2.txt", SAMPLE_DATA)
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi\L1F2")
        File.WriteAllText(Path.GetTempPath & "Multi\L1F2\file3.txt", SAMPLE_DATA)
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi\L1F1\L2F1")
        File.WriteAllText(Path.GetTempPath & "Multi\L1F1\L2F1\file4.txt", SAMPLE_DATA)

        ' Backup directory
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Backup")
        _destinationPath = Path.GetTempPath & "Backup"

        ' Worker
        _backupAgent = New BackupAgent(Me)

        _filterList = New List(Of String)
    End Sub

    <TestCleanup()>
    Public Sub TestCleanup()
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Single", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Multi", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Backup", FileIO.DeleteDirectoryOption.DeleteAllContents)
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_FileList_SingleLevel()
        Dim fileList As List(Of String) = _backupAgent.GetFileList(Path.GetTempPath & "Single")

        Assert.AreEqual(2, fileList.Count, "Incorrect number of files returned.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Single\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Single\file2.txt"), "File 2 not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_FileList_MultiLevel()
        Dim fileList As List(Of String) = _backupAgent.GetFileList(Path.GetTempPath & "Multi")

        Assert.AreEqual(4, fileList.Count, "Incorrect number of files returned.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\L1F1\file2.txt"), "File 2 not found.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\L1F2\file3.txt"), "File 3 not found.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\L1F1\L2F1\file4.txt"), "File 4 not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_FileList_MultiLevel_Filters()
        _filterList.Add("...\L1F1")
        Dim fileList As List(Of String) = _backupAgent.GetFileList(Path.GetTempPath & "Multi")

        Assert.AreEqual(3, fileList.Count, "Incorrect number of files returned.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\L1F1\file2.txt"), "File 2 not found.")
        Assert.IsTrue(fileList.Contains(Path.GetTempPath & "Multi\L1F1\L2F1\file4.txt"), "File 4 not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_FullBackup_SingleLevel()
        _sourcePath = Path.GetTempPath & "Single"

        _backupAgent.RunFullBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.AreEqual(2, fileList.Count, "Incorrect number of files returned.")
        Assert.IsTrue(fileList.Contains(_destinationPath & "\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.Contains(_destinationPath & "\file2.txt"), "File 2 not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_FullBackup_MultiLevel()
        _sourcePath = Path.GetTempPath & "Multi"

        _backupAgent.RunFullBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.AreEqual(4, fileList.Count, "Incorrect number of files returned.")
        Assert.IsTrue(fileList.Contains(_destinationPath & "\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.Contains(_destinationPath & "\L1F1\file2.txt"), "File 2 not found.")
        Assert.IsTrue(fileList.Contains(_destinationPath & "\L1F2\file3.txt"), "File 3 not found.")
        Assert.IsTrue(fileList.Contains(_destinationPath & "\L1F1\L2F1\file4.txt"), "File 4 not found.")
    End Sub

End Class