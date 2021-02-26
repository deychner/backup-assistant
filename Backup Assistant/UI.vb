Imports BackupAssistant.Core
Imports System.Collections.ObjectModel
Imports System.Text

Public Class UI
    Implements IBackupStarter

    Private Enum BackupType As Integer
        Incremental = 0
        Full
    End Enum

    Private _backupAgent As BackupAgent
    Private _logMessage As StringBuilder

    Private _filterList As List(Of String)
    Friend ReadOnly Property Filters As ReadOnlyCollection(Of String) Implements IBackupStarter.Filters
        Get
            Return _filterList.AsReadOnly
        End Get
    End Property

    Private Delegate Sub UpdateProgressCallback(progress As Integer)
    Private Delegate Sub UpdateStatusCallback(status As String)
    Private Delegate Sub PreProcessCallback()
    Private Delegate Sub PostProcessCallback()

    Private Sub UI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not EventLog.SourceExists("Backup Assistant") Then
            HandleException(New NotSupportedException("You must establish an event log for this application."))
            Environment.Exit(1)
        End If

        _backupAgent = New BackupAgent(Me)
        _logMessage = New StringBuilder
        _filterList = New List(Of String)

        Me.UI_ToolStripStatus.Text = String.Empty

        LoadSettings()
    End Sub

    Private Sub LoadSettings()
        If String.IsNullOrEmpty(My.Settings.Source) Then
            ' Default source
            Me.UI_TextBox_Source.Text = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            My.Settings.Source = Me.UI_TextBox_Source.Text
        Else
            ' Load source
            Me.UI_TextBox_Source.Text = My.Settings.Source
        End If

        If Not String.IsNullOrEmpty(My.Settings.Destination) Then
            ' Load destination
            Me.UI_TextBox_Destination.Text = My.Settings.Destination
        End If

        ' Load backup type
        Me.UI_ComboBox_Type.SelectedIndex = My.Settings.BackupType

        ' Initialize filters
        If My.Settings.Filters Is Nothing Then
            My.Settings.Filters = New System.Collections.Specialized.StringCollection()
        End If

        ' Load filters
        For Each f As String In My.Settings.Filters
            _filterList.Add(f)
        Next

        UpdateFilterIcon()
    End Sub

#Region "Menu"

    Private Sub UI_MenuStrip_Exit_Click(sender As Object, e As EventArgs) Handles UI_MenuStrip_Exit.Click
        Environment.Exit(0)
    End Sub

    Private Sub UI_MenuStrip_About_Click(sender As Object, e As EventArgs) Handles UI_MenuStrip_About.Click
        Using about As Form = New About
            about.ShowDialog()
        End Using
    End Sub

#End Region

#Region "File browsers"

    Private Sub UI_Button_Source_Click(sender As Object, e As EventArgs) Handles UI_Button_Source.Click
        Dim directory As String = GetFolderFromUser()

        If Not String.IsNullOrEmpty(directory) AndAlso My.Computer.FileSystem.DirectoryExists(directory) Then
            Me.UI_TextBox_Source.Text = directory
            My.Settings.Source = directory
        End If
    End Sub

    Private Sub UI_Button_Destination_Click(sender As Object, e As EventArgs) Handles UI_Button_Destination.Click
        Dim directory As String = GetFolderFromUser()

        If Not String.IsNullOrEmpty(directory) AndAlso My.Computer.FileSystem.DirectoryExists(directory) Then
            Me.UI_TextBox_Destination.Text = directory
            My.Settings.Destination = directory
        End If
    End Sub

    Private Sub UI_Button_Filter_Click(sender As Object, e As EventArgs) Handles UI_Button_Filter.Click
        Using dialog As New FilterSelection(Me)
            If dialog.ShowDialog = DialogResult.OK Then
                ' Update internal collection
                _filterList = dialog.GetFilterList

                ' Update settings
                My.Settings.Filters.Clear()
                My.Settings.Filters.AddRange(_filterList.ToArray)
            End If
        End Using

        UpdateFilterIcon()
    End Sub

    Private Sub UI_TextBox_Source_TextChanged(sender As Object, e As EventArgs) Handles UI_TextBox_Source.TextChanged
        Me.UI_Button_Filter.Enabled = Not String.IsNullOrEmpty(Me.UI_TextBox_Source.Text)
    End Sub

    Private Sub UpdateFilterIcon()
        If My.Settings.Filters.Count = 0 Then
            Me.UI_Button_Filter.Image = My.Resources.Resources.filter
        Else
            Me.UI_Button_Filter.Image = My.Resources.Resources.filter_apply
        End If
    End Sub

    Private Shared Function GetFolderFromUser() As String
        Using dialog As New FolderBrowserDialog
            dialog.SelectedPath = "C:\"
            dialog.ShowNewFolderButton = False

            If dialog.ShowDialog = DialogResult.OK Then
                Return dialog.SelectedPath
            Else
                Return String.Empty
            End If
        End Using
    End Function

#End Region

#Region "Backup"

    Public ReadOnly Property Source As String Implements IBackupStarter.SourcePath
        Get
            Return My.Settings.Source
        End Get
    End Property

    Public ReadOnly Property Destination As String Implements IBackupStarter.DestinationPath
        Get
            Return My.Settings.Destination
        End Get
    End Property

    Private Sub UI_ComboBox_Type_SelectedIndexChanged(sender As Object, e As EventArgs) Handles UI_ComboBox_Type.SelectedIndexChanged
        My.Settings.BackupType = Me.UI_ComboBox_Type.SelectedIndex
    End Sub

    Private Sub UI_Button_Backup_Click(sender As Object, e As EventArgs) Handles UI_Button_Backup.Click
        _logMessage.Clear()

        Try
            Select Case My.Settings.BackupType
                Case BackupType.Incremental
                    _backupAgent.RunIncrementalBackup()
                Case BackupType.Full
                    _backupAgent.RunFullBackup()
                Case Else
                    ' Do nothing
            End Select
        Catch operation As InvalidOperationException
            HandleException(operation)
        Catch argument As ArgumentException
            HandleException(argument)
        Finally
            WriteLogEntry()
        End Try
    End Sub

    Private Sub UI_Button_Cancel_Click(sender As Object, e As EventArgs) Handles UI_Button_Cancel.Click
        _backupAgent.Cancel()
    End Sub

    Private Sub HandleException(ex As Exception)
        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

#End Region

#Region "Reporting"

    Friend Sub PreProcess() Implements IBackupStarter.PreProcess
        If Me.InvokeRequired Then
            Dim d As New PreProcessCallback(AddressOf PreProcessInternal)
            Me.Invoke(d)
        Else
            PreProcessInternal()
        End If
    End Sub

    Private Sub PreProcessInternal()
        UI_Button_Backup.Enabled = False
    End Sub

    Friend Sub PostProcess() Implements IBackupStarter.PostProcess
        If Me.InvokeRequired Then
            Dim d As New PostProcessCallback(AddressOf PostProcessInternal)
            Me.Invoke(d)
        Else
            PostProcessInternal()
        End If
    End Sub

    Private Sub PostProcessInternal()
        UI_Button_Backup.Enabled = True
    End Sub

    Protected Friend Sub UpdateProgress(progress As Integer) Implements IBackupStarter.ReportProgress
        If Me.InvokeRequired Then
            Dim d As New UpdateProgressCallback(AddressOf UpdateProgressInternal)
            Me.Invoke(d, New Object() {progress})
        Else
            UpdateProgressInternal(progress)
        End If
    End Sub

    Private Sub UpdateProgressInternal(progress As Integer)
        Me.UI_ToolStripProgressBar.Value = progress
    End Sub

    Protected Friend Sub UpdateStatus(status As String) Implements IBackupStarter.ReportStatus
        If Me.InvokeRequired Then
            Dim d As New UpdateStatusCallback(AddressOf UpdateStatusInternal)
            Me.Invoke(d, New Object() {status})
        Else
            UpdateStatusInternal(status)
        End If
    End Sub

    Private Sub UpdateStatusInternal(status As String)
        Me.UI_ToolStripStatus.Text = status
    End Sub

#End Region

#Region "Logging"

    Private Sub AddToLogEntry(message As String) Implements IBackupStarter.AddToLogEntry
        If _logMessage IsNot Nothing Then
            _logMessage.AppendLine(message)
        End If
    End Sub

    Private Sub WriteLogEntry()
        If _logMessage Is Nothing OrElse _logMessage.Length = 0 Then Exit Sub

        Using log As New EventLog
            log.Source = "Backup Assistant"

            log.WriteEntry(_logMessage.ToString, EventLogEntryType.Error)
        End Using
    End Sub

#End Region

End Class
