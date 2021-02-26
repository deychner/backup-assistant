Public Class About

    ''' <summary>
    ''' Creates a new About form.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()

        lbl_ApplicationName.Text = Application.ProductName
        lbl_Version.Text = "Version " & Application.ProductVersion.Substring(0, Application.ProductVersion.Length - 2)
    End Sub

    Private Sub btn_OK_Click(sender As System.Object, e As System.EventArgs) Handles btn_OK.Click
        Me.Close()
    End Sub
End Class