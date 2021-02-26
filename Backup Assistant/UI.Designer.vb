<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UI
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
        Dim UI_MenuStrip_File As System.Windows.Forms.ToolStripMenuItem
        Dim UI_MenuStrip_Help As System.Windows.Forms.ToolStripMenuItem
        Dim UI_GroupBox_Backup As System.Windows.Forms.GroupBox
        Dim UI_Label_Type As System.Windows.Forms.Label
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UI))
        Dim UI_Label_Destination As System.Windows.Forms.Label
        Dim UI_Label_Source As System.Windows.Forms.Label
        Dim UI_StatusStrip As System.Windows.Forms.StatusStrip
        Me.UI_MenuStrip_Exit = New System.Windows.Forms.ToolStripMenuItem()
        Me.UI_MenuStrip_About = New System.Windows.Forms.ToolStripMenuItem()
        Me.UI_Button_Filter = New System.Windows.Forms.Button()
        Me.UI_Button_Backup = New System.Windows.Forms.Button()
        Me.UI_Button_Cancel = New System.Windows.Forms.Button()
        Me.UI_ComboBox_Type = New System.Windows.Forms.ComboBox()
        Me.UI_Button_Destination = New System.Windows.Forms.Button()
        Me.UI_Button_Source = New System.Windows.Forms.Button()
        Me.UI_TextBox_Destination = New System.Windows.Forms.TextBox()
        Me.UI_TextBox_Source = New System.Windows.Forms.TextBox()
        Me.UI_ToolStripStatusLabel_Progress = New System.Windows.Forms.ToolStripStatusLabel()
        Me.UI_ToolStripProgressBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.UI_ToolStripStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.UI_MenuStrip = New System.Windows.Forms.MenuStrip()
        UI_MenuStrip_File = New System.Windows.Forms.ToolStripMenuItem()
        UI_MenuStrip_Help = New System.Windows.Forms.ToolStripMenuItem()
        UI_GroupBox_Backup = New System.Windows.Forms.GroupBox()
        UI_Label_Type = New System.Windows.Forms.Label()
        UI_Label_Destination = New System.Windows.Forms.Label()
        UI_Label_Source = New System.Windows.Forms.Label()
        UI_StatusStrip = New System.Windows.Forms.StatusStrip()
        UI_GroupBox_Backup.SuspendLayout()
        UI_StatusStrip.SuspendLayout()
        Me.UI_MenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'UI_MenuStrip_File
        '
        UI_MenuStrip_File.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UI_MenuStrip_Exit})
        UI_MenuStrip_File.Name = "UI_MenuStrip_File"
        UI_MenuStrip_File.Size = New System.Drawing.Size(37, 20)
        UI_MenuStrip_File.Text = "File"
        '
        'UI_MenuStrip_Exit
        '
        Me.UI_MenuStrip_Exit.Name = "UI_MenuStrip_Exit"
        Me.UI_MenuStrip_Exit.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.F4), System.Windows.Forms.Keys)
        Me.UI_MenuStrip_Exit.Size = New System.Drawing.Size(134, 22)
        Me.UI_MenuStrip_Exit.Text = "Exit"
        '
        'UI_MenuStrip_Help
        '
        UI_MenuStrip_Help.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UI_MenuStrip_About})
        UI_MenuStrip_Help.Name = "UI_MenuStrip_Help"
        UI_MenuStrip_Help.Size = New System.Drawing.Size(44, 20)
        UI_MenuStrip_Help.Text = "Help"
        '
        'UI_MenuStrip_About
        '
        Me.UI_MenuStrip_About.Image = Global.BackupAssistant.My.Resources.Resources.help
        Me.UI_MenuStrip_About.Name = "UI_MenuStrip_About"
        Me.UI_MenuStrip_About.ShortcutKeys = System.Windows.Forms.Keys.F1
        Me.UI_MenuStrip_About.Size = New System.Drawing.Size(126, 22)
        Me.UI_MenuStrip_About.Text = "About"
        '
        'UI_GroupBox_Backup
        '
        UI_GroupBox_Backup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        UI_GroupBox_Backup.Controls.Add(Me.UI_Button_Filter)
        UI_GroupBox_Backup.Controls.Add(Me.UI_Button_Backup)
        UI_GroupBox_Backup.Controls.Add(Me.UI_Button_Cancel)
        UI_GroupBox_Backup.Controls.Add(Me.UI_ComboBox_Type)
        UI_GroupBox_Backup.Controls.Add(UI_Label_Type)
        UI_GroupBox_Backup.Controls.Add(Me.UI_Button_Destination)
        UI_GroupBox_Backup.Controls.Add(Me.UI_Button_Source)
        UI_GroupBox_Backup.Controls.Add(UI_Label_Destination)
        UI_GroupBox_Backup.Controls.Add(Me.UI_TextBox_Destination)
        UI_GroupBox_Backup.Controls.Add(UI_Label_Source)
        UI_GroupBox_Backup.Controls.Add(Me.UI_TextBox_Source)
        UI_GroupBox_Backup.Location = New System.Drawing.Point(12, 27)
        UI_GroupBox_Backup.Name = "UI_GroupBox_Backup"
        UI_GroupBox_Backup.Size = New System.Drawing.Size(418, 127)
        UI_GroupBox_Backup.TabIndex = 1
        UI_GroupBox_Backup.TabStop = False
        UI_GroupBox_Backup.Text = "Backup"
        '
        'UI_Button_Filter
        '
        Me.UI_Button_Filter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UI_Button_Filter.Image = Global.BackupAssistant.My.Resources.Resources.filter
        Me.UI_Button_Filter.Location = New System.Drawing.Point(389, 17)
        Me.UI_Button_Filter.Name = "UI_Button_Filter"
        Me.UI_Button_Filter.Size = New System.Drawing.Size(23, 49)
        Me.UI_Button_Filter.TabIndex = 10
        Me.UI_Button_Filter.UseVisualStyleBackColor = True
        '
        'UI_Button_Backup
        '
        Me.UI_Button_Backup.Location = New System.Drawing.Point(198, 98)
        Me.UI_Button_Backup.Name = "UI_Button_Backup"
        Me.UI_Button_Backup.Size = New System.Drawing.Size(75, 23)
        Me.UI_Button_Backup.TabIndex = 9
        Me.UI_Button_Backup.Text = "Backup"
        Me.UI_Button_Backup.UseVisualStyleBackColor = True
        '
        'UI_Button_Cancel
        '
        Me.UI_Button_Cancel.Location = New System.Drawing.Point(279, 98)
        Me.UI_Button_Cancel.Name = "UI_Button_Cancel"
        Me.UI_Button_Cancel.Size = New System.Drawing.Size(75, 23)
        Me.UI_Button_Cancel.TabIndex = 8
        Me.UI_Button_Cancel.Text = "Cancel"
        Me.UI_Button_Cancel.UseVisualStyleBackColor = True
        '
        'UI_ComboBox_Type
        '
        Me.UI_ComboBox_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.UI_ComboBox_Type.FormattingEnabled = True
        Me.UI_ComboBox_Type.Items.AddRange(New Object() {"Incremental", "Full"})
        Me.UI_ComboBox_Type.Location = New System.Drawing.Point(75, 71)
        Me.UI_ComboBox_Type.Name = "UI_ComboBox_Type"
        Me.UI_ComboBox_Type.Size = New System.Drawing.Size(279, 21)
        Me.UI_ComboBox_Type.TabIndex = 7
        '
        'UI_Label_Type
        '
        UI_Label_Type.AutoSize = True
        UI_Label_Type.Location = New System.Drawing.Point(35, 74)
        UI_Label_Type.Name = "UI_Label_Type"
        UI_Label_Type.Size = New System.Drawing.Size(34, 13)
        UI_Label_Type.TabIndex = 6
        UI_Label_Type.Text = "Type:"
        '
        'UI_Button_Destination
        '
        Me.UI_Button_Destination.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UI_Button_Destination.Image = CType(resources.GetObject("UI_Button_Destination.Image"), System.Drawing.Image)
        Me.UI_Button_Destination.Location = New System.Drawing.Point(360, 43)
        Me.UI_Button_Destination.Name = "UI_Button_Destination"
        Me.UI_Button_Destination.Size = New System.Drawing.Size(23, 23)
        Me.UI_Button_Destination.TabIndex = 5
        Me.UI_Button_Destination.UseVisualStyleBackColor = True
        '
        'UI_Button_Source
        '
        Me.UI_Button_Source.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UI_Button_Source.Image = CType(resources.GetObject("UI_Button_Source.Image"), System.Drawing.Image)
        Me.UI_Button_Source.Location = New System.Drawing.Point(360, 17)
        Me.UI_Button_Source.Name = "UI_Button_Source"
        Me.UI_Button_Source.Size = New System.Drawing.Size(23, 23)
        Me.UI_Button_Source.TabIndex = 4
        Me.UI_Button_Source.UseVisualStyleBackColor = True
        '
        'UI_Label_Destination
        '
        UI_Label_Destination.AutoSize = True
        UI_Label_Destination.Location = New System.Drawing.Point(6, 48)
        UI_Label_Destination.Name = "UI_Label_Destination"
        UI_Label_Destination.Size = New System.Drawing.Size(63, 13)
        UI_Label_Destination.TabIndex = 3
        UI_Label_Destination.Text = "Destination:"
        '
        'UI_TextBox_Destination
        '
        Me.UI_TextBox_Destination.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UI_TextBox_Destination.Enabled = False
        Me.UI_TextBox_Destination.Location = New System.Drawing.Point(75, 45)
        Me.UI_TextBox_Destination.Name = "UI_TextBox_Destination"
        Me.UI_TextBox_Destination.Size = New System.Drawing.Size(279, 20)
        Me.UI_TextBox_Destination.TabIndex = 2
        '
        'UI_Label_Source
        '
        UI_Label_Source.AutoSize = True
        UI_Label_Source.Location = New System.Drawing.Point(25, 22)
        UI_Label_Source.Name = "UI_Label_Source"
        UI_Label_Source.Size = New System.Drawing.Size(44, 13)
        UI_Label_Source.TabIndex = 1
        UI_Label_Source.Text = "Source:"
        '
        'UI_TextBox_Source
        '
        Me.UI_TextBox_Source.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.UI_TextBox_Source.Enabled = False
        Me.UI_TextBox_Source.Location = New System.Drawing.Point(75, 19)
        Me.UI_TextBox_Source.Name = "UI_TextBox_Source"
        Me.UI_TextBox_Source.Size = New System.Drawing.Size(279, 20)
        Me.UI_TextBox_Source.TabIndex = 0
        '
        'UI_StatusStrip
        '
        UI_StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UI_ToolStripStatusLabel_Progress, Me.UI_ToolStripProgressBar, Me.UI_ToolStripStatus})
        UI_StatusStrip.Location = New System.Drawing.Point(0, 157)
        UI_StatusStrip.Name = "UI_StatusStrip"
        UI_StatusStrip.Size = New System.Drawing.Size(442, 22)
        UI_StatusStrip.TabIndex = 2
        '
        'UI_ToolStripStatusLabel_Progress
        '
        Me.UI_ToolStripStatusLabel_Progress.Name = "UI_ToolStripStatusLabel_Progress"
        Me.UI_ToolStripStatusLabel_Progress.Size = New System.Drawing.Size(55, 17)
        Me.UI_ToolStripStatusLabel_Progress.Text = "Progress:"
        '
        'UI_ToolStripProgressBar
        '
        Me.UI_ToolStripProgressBar.Name = "UI_ToolStripProgressBar"
        Me.UI_ToolStripProgressBar.Size = New System.Drawing.Size(100, 16)
        '
        'UI_ToolStripStatus
        '
        Me.UI_ToolStripStatus.Name = "UI_ToolStripStatus"
        Me.UI_ToolStripStatus.Size = New System.Drawing.Size(47, 17)
        Me.UI_ToolStripStatus.Text = "Current"
        '
        'UI_MenuStrip
        '
        Me.UI_MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {UI_MenuStrip_File, UI_MenuStrip_Help})
        Me.UI_MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.UI_MenuStrip.Name = "UI_MenuStrip"
        Me.UI_MenuStrip.Size = New System.Drawing.Size(442, 24)
        Me.UI_MenuStrip.TabIndex = 0
        Me.UI_MenuStrip.Text = "MenuStrip1"
        '
        'UI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(442, 179)
        Me.Controls.Add(UI_StatusStrip)
        Me.Controls.Add(UI_GroupBox_Backup)
        Me.Controls.Add(Me.UI_MenuStrip)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.UI_MenuStrip
        Me.MaximumSize = New System.Drawing.Size(600, 217)
        Me.MinimumSize = New System.Drawing.Size(458, 217)
        Me.Name = "UI"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Backup Assistant"
        UI_GroupBox_Backup.ResumeLayout(False)
        UI_GroupBox_Backup.PerformLayout()
        UI_StatusStrip.ResumeLayout(False)
        UI_StatusStrip.PerformLayout()
        Me.UI_MenuStrip.ResumeLayout(False)
        Me.UI_MenuStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents UI_MenuStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents UI_MenuStrip_Exit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UI_MenuStrip_About As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UI_ToolStripStatusLabel_Progress As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents UI_ToolStripProgressBar As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents UI_ToolStripStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents UI_TextBox_Destination As System.Windows.Forms.TextBox
    Friend WithEvents UI_TextBox_Source As System.Windows.Forms.TextBox
    Friend WithEvents UI_Button_Destination As System.Windows.Forms.Button
    Friend WithEvents UI_Button_Source As System.Windows.Forms.Button
    Friend WithEvents UI_ComboBox_Type As System.Windows.Forms.ComboBox
    Friend WithEvents UI_Button_Backup As System.Windows.Forms.Button
    Friend WithEvents UI_Button_Cancel As System.Windows.Forms.Button
    Friend WithEvents UI_Button_Filter As System.Windows.Forms.Button

End Class
