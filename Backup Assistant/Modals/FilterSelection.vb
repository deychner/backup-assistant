Imports System.Collections.ObjectModel

Friend Class FilterSelection

    Public Sub New(caller As UI)
        InitializeComponent()

        ' Populate list
        If Not String.IsNullOrEmpty(caller.Source) Then
            ' Get directories below Source
            Dim directoriesToFilter As ReadOnlyCollection(Of String)
            Try
                directoriesToFilter = My.Computer.FileSystem.GetDirectories(caller.Source)
            Catch ex As UnauthorizedAccessException
                directoriesToFilter = New ReadOnlyCollection(Of String)({})
            End Try

            ' Add to list
            For Each d In directoriesToFilter
                Dim shortName As String = d.Replace(caller.Source, "...")
                Me.CheckedListBox_Filters.Items.Add(shortName, caller.Filters.Contains(shortName))
            Next
        End If
    End Sub

    Public Function GetFilterList() As IList(Of String)
        Dim filters As New List(Of String)

        For Each d In Me.CheckedListBox_Filters.CheckedItems
            filters.Add(d.ToString)
        Next

        Return filters
    End Function

End Class