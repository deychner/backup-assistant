Imports BackupAssistant.Core
Imports System.IO
Imports System.Collections.ObjectModel

<TestClass()>
Public Class BackupAgent_Validation
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

    <TestMethod()>
    Public Sub BackupAgent_Validate_Full_CallerNull()
        Try
            _backupAgent = New BackupAgent(Nothing)
            _backupAgent.RunFullBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentNullException
            If Not ex.Message.Contains("Parameter name: caller") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Full_SourcePathNull()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = String.Empty
        _destinationPath = Path.GetTempPath

        Try
            _backupAgent.RunFullBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("You must specify a backup source.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Full_SourcePathInvalid()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = Guid.NewGuid.ToString
        _destinationPath = Path.GetTempPath

        Try
            _backupAgent.RunFullBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("The specified source directory could not be found.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Full_DestinationPathNull()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = Path.GetTempPath
        _destinationPath = String.Empty

        Try
            _backupAgent.RunFullBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("You must specify a backup destination.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Full_DestinationPathInvalid()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = Path.GetTempPath
        _destinationPath = Guid.NewGuid.ToString

        Try
            _backupAgent.RunFullBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("The specified destination directory could not be found.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Incremental_CallerNull()
        Try
            _backupAgent = New BackupAgent(Nothing)
            _backupAgent.RunIncrementalBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentNullException
            If Not ex.Message.Contains("Parameter name: caller") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Incremental_SourcePathNull()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = String.Empty
        _destinationPath = Path.GetTempPath

        Try
            _backupAgent.RunIncrementalBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("You must specify a backup source.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Incremental_SourcePathInvalid()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = Guid.NewGuid.ToString
        _destinationPath = Path.GetTempPath

        Try
            _backupAgent.RunIncrementalBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("The specified source directory could not be found.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Incremental_DestinationPathNull()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = Path.GetTempPath
        _destinationPath = String.Empty

        Try
            _backupAgent.RunIncrementalBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("You must specify a backup destination.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

    <TestMethod()>
    Public Sub BackupAgent_Validate_Incremental_DestinationPathInvalid()
        _backupAgent = New BackupAgent(Me)
        _sourcePath = Path.GetTempPath
        _destinationPath = Guid.NewGuid.ToString

        Try
            _backupAgent.RunIncrementalBackupInternal()

            Assert.Fail("Expected exception was not generated.")
        Catch ex As ArgumentException
            If Not ex.Message.Contains("The specified destination directory could not be found.") Then
                Assert.Fail("Unexpected exception encountered.")
            End If
        End Try
    End Sub

End Class