Imports System.Threading
Imports System.Collections.ObjectModel
Imports System.IO

Namespace Core

    Friend Class BackupAgent

        Private _caller As IBackupStarter

        Private _cancelOperation As Boolean = False

        Public Sub New(caller As IBackupStarter)
            If caller Is Nothing Then Throw New ArgumentNullException("caller")

            _caller = caller
        End Sub

        Private Sub ValidateBackup()
            If String.IsNullOrEmpty(_caller.SourcePath) Then
                Throw New ArgumentException("You must specify a backup source.")
            End If

            If Not My.Computer.FileSystem.DirectoryExists(_caller.SourcePath) Then
                Throw New ArgumentException("The specified source directory could not be found.")
            End If

            If String.IsNullOrEmpty(_caller.DestinationPath) Then
                Throw New ArgumentException("You must specify a backup destination.")
            End If

            If Not My.Computer.FileSystem.DirectoryExists(_caller.DestinationPath) Then
                Throw New ArgumentException("The specified destination directory could not be found.")
            End If
        End Sub

        Public Sub Cancel()
            _cancelOperation = True
        End Sub

#Region "Full backup"

        Public Sub RunFullBackup()
            Dim backup As New Thread(AddressOf RunFullBackupInternal)
            backup.IsBackground = True
            backup.Start()
        End Sub

        Friend Sub RunFullBackupInternal()
            _caller.PreProcess()

            ValidateBackup()

            _caller.ReportProgress(0)
            _cancelOperation = False

            ' Get file list
            _caller.ReportStatus("Getting source file list...")
            Dim sourceFiles As List(Of String) = GetFileList(_caller.SourcePath)

            ' Delete destination directory
            _caller.ReportStatus("Deleting destination directory...")

            If My.Computer.FileSystem.DirectoryExists(_caller.DestinationPath) Then
                My.Computer.FileSystem.DeleteDirectory(_caller.DestinationPath, FileIO.DeleteDirectoryOption.DeleteAllContents)
            End If

            ' Copy files
            _caller.ReportStatus("Copying files...")
            For i As Integer = 0 To sourceFiles.Count - 1
                ' Check for cancellation
                If _cancelOperation Then
                    Exit For
                End If

                Dim destinationFile As String = sourceFiles.Item(i).Replace(_caller.SourcePath, _caller.DestinationPath)

                SafeCopyFile(sourceFiles.Item(i), destinationFile)

                _caller.ReportProgress(100 * (i + 1) / sourceFiles.Count)
            Next

            If _cancelOperation Then
                _caller.ReportStatus("Backup was canceled.")
            Else
                _caller.ReportStatus("Backup is complete.")
            End If

            _caller.PostProcess()
        End Sub

#Region "Search"

        Friend Function GetFileList(rootDirectory As String) As ICollection(Of String)
            Dim files As New List(Of String)

            If _caller.Filters.Count > 0 Then
                ' Get files in root directory
                GetFilesInDirectory(rootDirectory, files)

                ' Get files in filtered directories and all subdirectories
                For Each f In _caller.Filters
                    ' Each filter directory is a relative path
                    Dim d As String = GetFullFileName(f, rootDirectory)
                    DirectorySearch(d, files)
                Next
            Else
                DirectorySearch(rootDirectory, files)
            End If

            Return files
        End Function

        Private Sub DirectorySearch(directory As String, fileList As ICollection(Of String))
            GetFilesInDirectory(directory, fileList)

            For Each d As String In SafeGetDirectories(directory)
                DirectorySearch(d, fileList)
            Next
        End Sub

        Private Sub GetFilesInDirectory(directory As String, fileList As ICollection(Of String))
            For Each f In SafeGetFiles(directory)
                fileList.Add(f)
            Next
        End Sub

#End Region

#End Region

#Region "Incremental backup"

        Public Sub RunIncrementalBackup()
            Dim backup As New Thread(AddressOf RunIncrementalBackupInternal)
            backup.IsBackground = True
            backup.Start()
        End Sub

        Public Sub RunIncrementalBackupInternal()
            _caller.PreProcess()

            ValidateBackup()

            _caller.ReportProgress(0)
            _cancelOperation = False

            ' Get file list
            _caller.ReportStatus("Getting file information...")
            Dim files As Dictionary(Of String, FileListing) = GetCombinedFileList(_caller.SourcePath, _caller.DestinationPath)

            ' Process files
            _caller.ReportStatus("Processing files...")

            ' Get total size
            Dim processed As Long = 0
            Dim totalSize As Long = (From f In files.Values
                                     Where f.GetBackupAction <> BackupAction.None
                                     Select f.Size).Sum

            For Each key In files.Keys
                ' Check for cancellation
                If _cancelOperation Then
                    Exit For
                End If

                Dim listing As FileListing = files(key)

                Select Case listing.GetBackupAction
                    Case BackupAction.None
                        ' Do nothing
                    Case BackupAction.Copy
                        Dim sourceFile As String = ExpandSourceFileName(key)
                        Dim destinationFile As String = ExpandDestinationFileName(key)

                        SafeCopyFile(sourceFile, destinationFile, False)
                        processed += listing.Size
                    Case BackupAction.Overwrite
                        Dim sourceFile As String = ExpandSourceFileName(key)
                        Dim destinationFile As String = ExpandDestinationFileName(key)

                        SafeCopyFile(sourceFile, destinationFile, True)
                        processed += listing.Size
                    Case BackupAction.Delete
                        Dim destinationFile As String = ExpandDestinationFileName(key)

                        SafeDeleteFile(destinationFile)
                        processed += listing.Size
                    Case Else
                        ' Do nothing
                End Select

                _caller.ReportProgress(100L * (processed / totalSize))
            Next

            If _cancelOperation Then
                _caller.ReportStatus("Backup was canceled.")
            Else
                _caller.ReportStatus("Backup is complete.")
            End If

            _caller.PostProcess()
        End Sub

#Region "Search"

        Friend Function GetCombinedFileList(sourceDirectory As String, destinationDirectory As String) As IDictionary(Of String, FileListing)
            Dim files As New Dictionary(Of String, FileListing)

            If _caller.Filters.Count > 0 Then
                ' Get files in root source directory
                _caller.ReportStatus("Getting file information for source directory...")
                CollectFilesForSourceListing(sourceDirectory, files)

                ' Get files in filtered source directories and all subdirectories
                For Each f In _caller.Filters
                    ' Each filter directory is a relative path
                    Dim d As String = GetFullFileName(f, sourceDirectory)
                    SourceDirectorySearch(d, files)
                Next

                ' Get files in root destination directory
                _caller.ReportStatus("Getting file information for destination directory...")
                CollectFilesForDestinationListing(destinationDirectory, files)

                ' Get files in filtered source directories and all subdirectories
                For Each f In _caller.Filters
                    ' Each filter directory is a relative path
                    Dim d As String = GetFullFileName(f, destinationDirectory)
                    DestinationDirectorySearch(d, files)
                Next
            Else
                _caller.ReportStatus("Getting file information for source directory...")
                SourceDirectorySearch(sourceDirectory, files)

                _caller.ReportStatus("Getting file information for destination directory...")
                DestinationDirectorySearch(destinationDirectory, files)
            End If

            Return files
        End Function

        Private Sub SourceDirectorySearch(directory As String, fileList As IDictionary(Of String, FileListing))
            CollectFilesForSourceListing(directory, fileList)

            For Each d As String In SafeGetDirectories(directory)
                SourceDirectorySearch(d, fileList)
            Next
        End Sub

        Private Sub CollectFilesForSourceListing(directory As String, fileList As IDictionary(Of String, FileListing))
            For Each f In SafeGetFiles(directory)
                Dim fileName As String = ShrinkSourceFileName(f)
                Dim info As FileInfo = SafeGetFileInfo(f)

                If info IsNot Nothing Then
                    If fileList.ContainsKey(fileName) Then
                        fileList.Item(fileName).IsInSource = True
                        fileList.Item(fileName).SourceLastModified = info.LastWriteTime
                    Else
                        Dim listing As New FileListing
                        listing.IsInSource = True
                        listing.SourceLastModified = info.LastWriteTime
                        listing.Size = info.Length

                        fileList.Add(New KeyValuePair(Of String, FileListing)(fileName, listing))
                    End If
                End If
            Next
        End Sub

        Private Sub DestinationDirectorySearch(directory As String, fileList As IDictionary(Of String, FileListing))
            CollectFilesForDestinationListing(directory, fileList)

            For Each d As String In SafeGetDirectories(directory)
                DestinationDirectorySearch(d, fileList)
            Next
        End Sub

        Private Sub CollectFilesForDestinationListing(directory As String, fileList As IDictionary(Of String, FileListing))
            For Each f In SafeGetFiles(directory)
                Dim fileName As String = ShrinkDestinationFileName(f)
                Dim info As FileInfo = SafeGetFileInfo(f)

                If info IsNot Nothing Then
                    If fileList.ContainsKey(fileName) Then
                        fileList.Item(fileName).IsInDestination = True
                        fileList.Item(fileName).DestinationLastModified = info.LastWriteTime
                    Else
                        Dim listing As New FileListing
                        listing.IsInDestination = True
                        listing.DestinationLastModified = info.LastWriteTime
                        listing.Size = info.Length

                        fileList.Add(New KeyValuePair(Of String, FileListing)(fileName, listing))
                    End If
                End If
            Next
        End Sub

#End Region

#End Region

#Region "Shrink/Expand"

        Private Function ShrinkSourceFileName(fileName As String) As String
            Return GetAbbreviatedFileName(fileName, _caller.SourcePath)
        End Function

        Private Function ShrinkDestinationFileName(fileName As String) As String
            Return GetAbbreviatedFileName(fileName, _caller.DestinationPath)
        End Function

        Private Function ExpandSourceFileName(fileName As String) As String
            Return GetFullFileName(fileName, _caller.SourcePath)
        End Function

        Private Function ExpandDestinationFileName(fileName As String) As String
            Return GetFullFileName(fileName, _caller.DestinationPath)
        End Function

        Private Function GetFullFileName(abbreviatedFileName As String, lengthenString As String)
            Return abbreviatedFileName.Replace("...", lengthenString)
        End Function

        Private Function GetAbbreviatedFileName(fileName As String, shortenString As String) As String
            Return fileName.Replace(shortenString, "...")
        End Function

#End Region

#Region "Safety"

        Friend Function SafeGetFileInfo(file As String) As IO.FileInfo
            Try
                Return My.Computer.FileSystem.GetFileInfo(file)
            Catch ex As Exception
                Dim errorMessage As String = String.Format("Failed to get file information for file '{0}'.", file)
                _caller.AddToLogEntry(errorMessage)

                Return Nothing
            End Try
        End Function

        Friend Function SafeGetFiles(directory As String) As ReadOnlyCollection(Of String)
            Try
                Return My.Computer.FileSystem.GetFiles(directory)
            Catch ex As Exception
                _caller.AddToLogEntry(String.Format("Failed to get files in directory '{0}'.", directory))

                Return New ReadOnlyCollection(Of String)({})
            End Try
        End Function

        Friend Function SafeGetDirectories(directory As String) As ReadOnlyCollection(Of String)
            Try
                Return My.Computer.FileSystem.GetDirectories(directory)
            Catch ex As Exception
                _caller.AddToLogEntry(String.Format("Failed to get directories in directory '{0}'.", directory))

                Return New ReadOnlyCollection(Of String)({})
            End Try
        End Function

        Friend Sub SafeCopyFile(sourceFileName As String, destinationFileName As String)
            SafeCopyFile(sourceFileName, destinationFileName, False)
        End Sub

        Friend Sub SafeCopyFile(sourceFileName As String, destinationFileName As String, overwrite As Boolean)
            Try
                My.Computer.FileSystem.CopyFile(sourceFileName, destinationFileName, overwrite)
            Catch ex As Exception
                _caller.AddToLogEntry(String.Format("Copy file failed for file '{0}'.", sourceFileName))
            End Try
        End Sub

        Friend Sub SafeDeleteFile(file As String)
            Try
                My.Computer.FileSystem.DeleteFile(file)
            Catch ex As Exception
                _caller.AddToLogEntry(String.Format("Delete file failed for file '{0}'.", file))
            End Try
        End Sub

#End Region

    End Class

End Namespace
