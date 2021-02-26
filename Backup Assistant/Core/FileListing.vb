Namespace Core

    Friend Enum BackupAction As Byte
        None
        Copy
        Delete
        Overwrite
    End Enum

    Friend Class FileListing

        Public Property IsInSource As Boolean
        Public Property IsInDestination As Boolean
        Public Property SourceLastModified As Date
        Public Property DestinationLastModified As Date
        Public Property Size As Long

        Public Sub New()
            Me.IsInSource = False
            Me.IsInDestination = False
            Me.SourceLastModified = Date.MinValue
            Me.DestinationLastModified = Date.MinValue
            Me.Size = 0L
        End Sub

        Public Function GetBackupAction() As BackupAction
            If Not IsInSource AndAlso Not IsInDestination Then
                Return BackupAction.None
            ElseIf IsInSource AndAlso Not IsInDestination Then
                Return BackupAction.Copy
            ElseIf Not IsInSource AndAlso IsInDestination Then
                Return BackupAction.Delete
            ElseIf IsInSource AndAlso IsInDestination AndAlso SourceLastModified > DestinationLastModified Then
                Return BackupAction.Overwrite
            Else ' IsInSource AndAlso IsInDestination AndAlso SourceLastModified <= DestinationLastModified
                Return BackupAction.None
            End If
        End Function

    End Class

End Namespace
