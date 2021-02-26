<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class About
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(About))
        Me.lbl_ApplicationName = New System.Windows.Forms.Label()
        Me.lbl_Version = New System.Windows.Forms.Label()
        Me.btn_OK = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lbl_ApplicationName
        '
        Me.lbl_ApplicationName.Font = New System.Drawing.Font("Microsoft Sans Serif", 24.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lbl_ApplicationName.Location = New System.Drawing.Point(12, 9)
        Me.lbl_ApplicationName.Name = "lbl_ApplicationName"
        Me.lbl_ApplicationName.Size = New System.Drawing.Size(303, 37)
        Me.lbl_ApplicationName.TabIndex = 0
        Me.lbl_ApplicationName.Text = "Name"
        Me.lbl_ApplicationName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lbl_Version
        '
        Me.lbl_Version.Location = New System.Drawing.Point(12, 56)
        Me.lbl_Version.Name = "lbl_Version"
        Me.lbl_Version.Size = New System.Drawing.Size(303, 13)
        Me.lbl_Version.TabIndex = 1
        Me.lbl_Version.Text = "Version"
        Me.lbl_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btn_OK
        '
        Me.btn_OK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btn_OK.Location = New System.Drawing.Point(124, 82)
        Me.btn_OK.Name = "btn_OK"
        Me.btn_OK.Size = New System.Drawing.Size(75, 23)
        Me.btn_OK.TabIndex = 2
        Me.btn_OK.Text = "OK"
        Me.btn_OK.UseVisualStyleBackColor = True
        '
        'frm_About
        '
        Me.AcceptButton = Me.btn_OK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(327, 117)
        Me.Controls.Add(Me.btn_OK)
        Me.Controls.Add(Me.lbl_Version)
        Me.Controls.Add(Me.lbl_ApplicationName)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frm_About"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "About"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lbl_ApplicationName As System.Windows.Forms.Label
    Friend WithEvents lbl_Version As System.Windows.Forms.Label
    Friend WithEvents btn_OK As System.Windows.Forms.Button
End Class
