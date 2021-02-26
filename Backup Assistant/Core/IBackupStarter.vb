Imports System.Collections.ObjectModel

Namespace Core

    Friend Interface IBackupStarter

        ReadOnly Property SourcePath As String

        ReadOnly Property DestinationPath As String

        ReadOnly Property Filters As ReadOnlyCollection(Of String)

        Sub PreProcess()

        Sub PostProcess()

        Sub ReportProgress(progress As Integer)

        Sub ReportStatus(status As String)

        Sub AddToLogEntry(message As String)

    End Interface

End Namespace
