Imports BackupAssistant.Core
Imports System.IO
Imports System.Collections.ObjectModel

<TestClass()>
Public Class BackupAgent_IncrementalBackup
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

        ' Single_Backup
        '   file2.txt
        '   file3.txt
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Single_Backup")
        File.WriteAllText(Path.GetTempPath & "Single_Backup\file2.txt", SAMPLE_DATA)
        File.WriteAllText(Path.GetTempPath & "Single_Backup\file3.txt", SAMPLE_DATA)

        ' Multi_Backup
        '   file1.txt
        '   L1F1
        '     file5.txt
        '     L2F1
        '       file4.txt
        '   L1F2
        '     file3.txt
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi_Backup")
        File.WriteAllText(Path.GetTempPath & "Multi_Backup\file1.txt", SAMPLE_DATA)
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi_Backup\L1F1")
        File.WriteAllText(Path.GetTempPath & "Multi_Backup\L1F1\file5.txt", SAMPLE_DATA)
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi_Backup\L1F2")
        File.WriteAllText(Path.GetTempPath & "Multi_Backup\L1F2\file3.txt", SAMPLE_DATA)
        My.Computer.FileSystem.CreateDirectory(Path.GetTempPath & "Multi_Backup\L1F1\L2F1")
        File.WriteAllText(Path.GetTempPath & "Multi_Backup\L1F1\L2F1\file4.txt", SAMPLE_DATA)

        ' Worker
        _backupAgent = New BackupAgent(Me)

        _filterList = New List(Of String)
    End Sub

    <TestCleanup()>
    Public Sub TestCleanup()
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Single", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Multi", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Single_Backup", FileIO.DeleteDirectoryOption.DeleteAllContents)
        My.Computer.FileSystem.DeleteDirectory(Path.GetTempPath & "Multi_Backup", FileIO.DeleteDirectoryOption.DeleteAllContents)
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_SingleLevel_SourceOnly()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(3, fileList.Count, "Incorrect number of files found.")
        Assert.IsTrue(fileList.ContainsKey("...\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.Item("...\file1.txt").IsInSource, "File 1 not properly marked as being in the source location.")
        Assert.IsFalse(fileList.Item("...\file1.txt").IsInDestination, "File 1 not properly marked as not being in the destination location.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_SingleLevel_Overlap()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(3, fileList.Count, "Incorrect number of files found.")
        Assert.IsTrue(fileList.ContainsKey("...\file2.txt"), "File 2 not found.")
        Assert.IsTrue(fileList.Item("...\file2.txt").IsInSource, "File 2 not properly marked as being in the source location.")
        Assert.IsTrue(fileList.Item("...\file2.txt").IsInDestination, "File 2 not properly marked as being in the destination location.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_SingleLevel_DestinationOnly()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(3, fileList.Count, "Incorrect number of files found.")
        Assert.IsTrue(fileList.ContainsKey("...\file3.txt"), "File 3 not found.")
        Assert.IsFalse(fileList.Item("...\file3.txt").IsInSource, "File 3 not properly marked as not being in the source location.")
        Assert.IsTrue(fileList.Item("...\file3.txt").IsInDestination, "File 3 not properly marked as being in the destination location.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_MultiLevel_SourceOnly()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(5, fileList.Count, "Incorrect number of files found.")
        Assert.IsTrue(fileList.ContainsKey("...\L1F1\file2.txt"), "File 2 not found.")
        Assert.IsTrue(fileList.Item("...\L1F1\file2.txt").IsInSource, "File 2 not properly marked as being in the source location.")
        Assert.IsFalse(fileList.Item("...\L1F1\file2.txt").IsInDestination, "File 2 not properly marked as not being in the destination location.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_MultiLevel_Overlap()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(5, fileList.Count, "Incorrect number of files found.")
        Assert.IsTrue(fileList.ContainsKey("...\L1F2\file3.txt"), "File 3 not found.")
        Assert.IsTrue(fileList.Item("...\L1F2\file3.txt").IsInSource, "File 3 not properly marked as being in the source location.")
        Assert.IsTrue(fileList.Item("...\L1F2\file3.txt").IsInDestination, "File 3 not properly marked as being in the destination location.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_MultiLevel_DestinationOnly()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(5, fileList.Count, "Incorrect number of files found.")
        Assert.IsTrue(fileList.ContainsKey("...\L1F1\file5.txt"), "File 5 not found.")
        Assert.IsFalse(fileList.Item("...\L1F1\file5.txt").IsInSource, "File 5 not properly marked as not being in the source location.")
        Assert.IsTrue(fileList.Item("...\L1F1\file5.txt").IsInDestination, "File 5 not properly marked as being in the destination location.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_CombinedFileList_MultiLevel_Filters()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"
        _filterList.Add("...\L1F1")

        Dim fileList As Dictionary(Of String, FileListing) = _backupAgent.GetCombinedFileList(_sourcePath, _destinationPath)

        Assert.AreEqual(4, fileList.Count, "Incorrect number of files returned.")
        Assert.IsTrue(fileList.ContainsKey("...\file1.txt"), "File 1 not found.")
        Assert.IsTrue(fileList.ContainsKey("...\L1F1\file2.txt"), "File 2 not found.")
        Assert.IsTrue(fileList.ContainsKey("...\L1F1\L2F1\file4.txt"), "File 4 not found.")
        Assert.IsTrue(fileList.ContainsKey("...\L1F1\file5.txt"), "File 5 not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_SingleLevel_NoAction()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        _backupAgent.RunIncrementalBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsTrue(fileList.Contains(_destinationPath & "\file2.txt"), "File 2 was not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_SingleLevel_Copy()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        _backupAgent.RunIncrementalBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsTrue(fileList.Contains(_destinationPath & "\file1.txt"), "File 1 was not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_SingleLevel_Delete()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        _backupAgent.RunIncrementalBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsFalse(fileList.Contains(_destinationPath & "\file3.txt"), "File 3 was found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_SingleLevel_Overwrite()
        _sourcePath = Path.GetTempPath & "Single"
        _destinationPath = Path.GetTempPath & "Single_Backup"

        ' Touch file3.txt in source to make it more recent
        File.WriteAllText(_sourcePath & "\file2.txt", "New content")

        ' Check dates
        Dim sourceLastModified As Date = My.Computer.FileSystem.GetFileInfo(_sourcePath & "\file2.txt").LastWriteTime
        Dim destinationLastModified As Date = My.Computer.FileSystem.GetFileInfo(_destinationPath & "\file2.txt").LastWriteTime

        If sourceLastModified < destinationLastModified Then
            Assert.Inconclusive("The test did not make a source file that is newer than the destination file.")
        End If

        _backupAgent.RunIncrementalBackupInternal()

        ' Refresh dates
        sourceLastModified = My.Computer.FileSystem.GetFileInfo(_sourcePath & "\file2.txt").LastWriteTime
        destinationLastModified = My.Computer.FileSystem.GetFileInfo(_destinationPath & "\file2.txt").LastWriteTime

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsTrue(fileList.Contains(_destinationPath & "\file2.txt"), "File 2 was not found.")
        Assert.IsTrue(destinationLastModified >= sourceLastModified, "File 2 was not updated.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_MultiLevel_NoAction()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        _backupAgent.RunIncrementalBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsTrue(fileList.Contains(_destinationPath & "\L1F2\file3.txt"), "File 3 was not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_MultiLevel_Copy()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        _backupAgent.RunIncrementalBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsTrue(fileList.Contains(_destinationPath & "\L1F1\file2.txt"), "File 5 was not found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_MultiLevel_Delete()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        _backupAgent.RunIncrementalBackupInternal()

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsFalse(fileList.Contains(_destinationPath & "\L1F1\file5.txt"), "File 5 was found.")
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_IncrementalBackup_MultiLevel_Overwrite()
        _sourcePath = Path.GetTempPath & "Multi"
        _destinationPath = Path.GetTempPath & "Multi_Backup"

        ' Touch file3.txt in source to make it more recent
        File.WriteAllText(_sourcePath & "\L1F2\file3.txt", "New content")

        ' Check dates
        Dim sourceLastModified As Date = My.Computer.FileSystem.GetFileInfo(_sourcePath & "\L1F2\file3.txt").LastWriteTime
        Dim destinationLastModified As Date = My.Computer.FileSystem.GetFileInfo(_destinationPath & "\L1F2\file3.txt").LastWriteTime

        If sourceLastModified < destinationLastModified Then
            Assert.Inconclusive("The test did not make a source file that is newer than the destination file.")
        End If

        _backupAgent.RunIncrementalBackupInternal()

        ' Refresh dates
        sourceLastModified = My.Computer.FileSystem.GetFileInfo(_sourcePath & "\L1F2\file3.txt").LastWriteTime
        destinationLastModified = My.Computer.FileSystem.GetFileInfo(_destinationPath & "\L1F2\file3.txt").LastWriteTime

        Dim fileList As List(Of String) = _backupAgent.GetFileList(_destinationPath)
        Assert.IsTrue(fileList.Contains(_destinationPath & "\L1F2\file3.txt"), "File 3 was not found.")
        Assert.IsTrue(destinationLastModified >= sourceLastModified, "File 3 was not updated.")
    End Sub

End Class