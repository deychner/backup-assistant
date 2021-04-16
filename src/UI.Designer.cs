
namespace BackupAssistant
{
    partial class UI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ToolStripMenuItem UI_MenuStrip_File;
            System.Windows.Forms.ToolStripMenuItem UI_MenuStrip_Help;
            System.Windows.Forms.StatusStrip UI_StatusStrip;
            System.Windows.Forms.GroupBox UI_GroupBox_Backup;
            System.Windows.Forms.Label UI_Label_Type;
            System.Windows.Forms.Label UI_Label_Source;
            System.Windows.Forms.Label UI_Label_Destination;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI));
            this.UI_MenuStrip_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.UI_MenuStrip_About = new System.Windows.Forms.ToolStripMenuItem();
            this.UI_ToolStripStatusLabel_Progress = new System.Windows.Forms.ToolStripStatusLabel();
            this.UI_ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.UI_ToolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.UI_Button_Backup = new System.Windows.Forms.Button();
            this.UI_Button_Cancel = new System.Windows.Forms.Button();
            this.UI_Button_Destination = new System.Windows.Forms.Button();
            this.UI_Button_Source = new System.Windows.Forms.Button();
            this.UI_Button_Filter = new System.Windows.Forms.Button();
            this.UI_ComboBox_Type = new System.Windows.Forms.ComboBox();
            this.UI_TextBox_Destination = new System.Windows.Forms.TextBox();
            this.UI_TextBox_Source = new System.Windows.Forms.TextBox();
            this.UI_MenuStrip = new System.Windows.Forms.MenuStrip();
            UI_MenuStrip_File = new System.Windows.Forms.ToolStripMenuItem();
            UI_MenuStrip_Help = new System.Windows.Forms.ToolStripMenuItem();
            UI_StatusStrip = new System.Windows.Forms.StatusStrip();
            UI_GroupBox_Backup = new System.Windows.Forms.GroupBox();
            UI_Label_Type = new System.Windows.Forms.Label();
            UI_Label_Source = new System.Windows.Forms.Label();
            UI_Label_Destination = new System.Windows.Forms.Label();
            UI_StatusStrip.SuspendLayout();
            UI_GroupBox_Backup.SuspendLayout();
            this.UI_MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // UI_MenuStrip_File
            // 
            UI_MenuStrip_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UI_MenuStrip_Exit});
            UI_MenuStrip_File.Name = "UI_MenuStrip_File";
            UI_MenuStrip_File.Size = new System.Drawing.Size(37, 20);
            UI_MenuStrip_File.Text = "File";
            // 
            // UI_MenuStrip_Exit
            // 
            this.UI_MenuStrip_Exit.Name = "UI_MenuStrip_Exit";
            this.UI_MenuStrip_Exit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.D4)));
            this.UI_MenuStrip_Exit.Size = new System.Drawing.Size(129, 22);
            this.UI_MenuStrip_Exit.Text = "Exit";
            // 
            // UI_MenuStrip_Help
            // 
            UI_MenuStrip_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UI_MenuStrip_About});
            UI_MenuStrip_Help.Name = "UI_MenuStrip_Help";
            UI_MenuStrip_Help.Size = new System.Drawing.Size(44, 20);
            UI_MenuStrip_Help.Text = "Help";
            // 
            // UI_MenuStrip_About
            // 
            this.UI_MenuStrip_About.Image = global::BackupAssistant.Properties.Resources.help;
            this.UI_MenuStrip_About.Name = "UI_MenuStrip_About";
            this.UI_MenuStrip_About.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.UI_MenuStrip_About.Size = new System.Drawing.Size(130, 26);
            this.UI_MenuStrip_About.Text = "About";
            // 
            // UI_StatusStrip
            // 
            UI_StatusStrip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            UI_StatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            UI_StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UI_ToolStripStatusLabel_Progress,
            this.UI_ToolStripProgressBar,
            this.UI_ToolStripStatus});
            UI_StatusStrip.Location = new System.Drawing.Point(0, 163);
            UI_StatusStrip.Name = "UI_StatusStrip";
            UI_StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            UI_StatusStrip.Size = new System.Drawing.Size(515, 22);
            UI_StatusStrip.TabIndex = 2;
            // 
            // UI_ToolStripStatusLabel_Progress
            // 
            this.UI_ToolStripStatusLabel_Progress.Name = "UI_ToolStripStatusLabel_Progress";
            this.UI_ToolStripStatusLabel_Progress.Size = new System.Drawing.Size(55, 17);
            this.UI_ToolStripStatusLabel_Progress.Text = "Progress:";
            // 
            // UI_ToolStripProgressBar
            // 
            this.UI_ToolStripProgressBar.Name = "UI_ToolStripProgressBar";
            this.UI_ToolStripProgressBar.Size = new System.Drawing.Size(175, 16);
            // 
            // UI_ToolStripStatus
            // 
            this.UI_ToolStripStatus.Name = "UI_ToolStripStatus";
            this.UI_ToolStripStatus.Size = new System.Drawing.Size(47, 17);
            this.UI_ToolStripStatus.Text = "Current";
            // 
            // UI_GroupBox_Backup
            // 
            UI_GroupBox_Backup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            UI_GroupBox_Backup.Controls.Add(this.UI_Button_Backup);
            UI_GroupBox_Backup.Controls.Add(this.UI_Button_Cancel);
            UI_GroupBox_Backup.Controls.Add(this.UI_Button_Destination);
            UI_GroupBox_Backup.Controls.Add(this.UI_Button_Source);
            UI_GroupBox_Backup.Controls.Add(this.UI_Button_Filter);
            UI_GroupBox_Backup.Controls.Add(this.UI_ComboBox_Type);
            UI_GroupBox_Backup.Controls.Add(UI_Label_Type);
            UI_GroupBox_Backup.Controls.Add(UI_Label_Source);
            UI_GroupBox_Backup.Controls.Add(UI_Label_Destination);
            UI_GroupBox_Backup.Controls.Add(this.UI_TextBox_Destination);
            UI_GroupBox_Backup.Controls.Add(this.UI_TextBox_Source);
            UI_GroupBox_Backup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            UI_GroupBox_Backup.Location = new System.Drawing.Point(12, 26);
            UI_GroupBox_Backup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            UI_GroupBox_Backup.Name = "UI_GroupBox_Backup";
            UI_GroupBox_Backup.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            UI_GroupBox_Backup.Size = new System.Drawing.Size(494, 135);
            UI_GroupBox_Backup.TabIndex = 3;
            UI_GroupBox_Backup.TabStop = false;
            UI_GroupBox_Backup.Text = "Backup";
            // 
            // UI_Button_Backup
            // 
            this.UI_Button_Backup.Location = new System.Drawing.Point(274, 106);
            this.UI_Button_Backup.Name = "UI_Button_Backup";
            this.UI_Button_Backup.Size = new System.Drawing.Size(75, 23);
            this.UI_Button_Backup.TabIndex = 10;
            this.UI_Button_Backup.Text = "Backup";
            this.UI_Button_Backup.UseVisualStyleBackColor = true;
            // 
            // UI_Button_Cancel
            // 
            this.UI_Button_Cancel.Location = new System.Drawing.Point(355, 106);
            this.UI_Button_Cancel.Name = "UI_Button_Cancel";
            this.UI_Button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.UI_Button_Cancel.TabIndex = 9;
            this.UI_Button_Cancel.Text = "Cancel";
            this.UI_Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // UI_Button_Destination
            // 
            this.UI_Button_Destination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UI_Button_Destination.Image = global::BackupAssistant.Properties.Resources.view;
            this.UI_Button_Destination.Location = new System.Drawing.Point(436, 47);
            this.UI_Button_Destination.Name = "UI_Button_Destination";
            this.UI_Button_Destination.Size = new System.Drawing.Size(23, 23);
            this.UI_Button_Destination.TabIndex = 8;
            this.UI_Button_Destination.UseVisualStyleBackColor = true;
            // 
            // UI_Button_Source
            // 
            this.UI_Button_Source.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UI_Button_Source.Image = global::BackupAssistant.Properties.Resources.view;
            this.UI_Button_Source.Location = new System.Drawing.Point(436, 20);
            this.UI_Button_Source.Name = "UI_Button_Source";
            this.UI_Button_Source.Size = new System.Drawing.Size(23, 23);
            this.UI_Button_Source.TabIndex = 7;
            this.UI_Button_Source.UseVisualStyleBackColor = true;
            // 
            // UI_Button_Filter
            // 
            this.UI_Button_Filter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UI_Button_Filter.Image = global::BackupAssistant.Properties.Resources.filter;
            this.UI_Button_Filter.Location = new System.Drawing.Point(465, 19);
            this.UI_Button_Filter.Name = "UI_Button_Filter";
            this.UI_Button_Filter.Size = new System.Drawing.Size(23, 52);
            this.UI_Button_Filter.TabIndex = 6;
            this.UI_Button_Filter.UseVisualStyleBackColor = true;
            // 
            // UI_ComboBox_Type
            // 
            this.UI_ComboBox_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UI_ComboBox_Type.FormattingEnabled = true;
            this.UI_ComboBox_Type.Items.AddRange(new object[] {
            "Incremental",
            "Full"});
            this.UI_ComboBox_Type.Location = new System.Drawing.Point(82, 77);
            this.UI_ComboBox_Type.Name = "UI_ComboBox_Type";
            this.UI_ComboBox_Type.Size = new System.Drawing.Size(348, 23);
            this.UI_ComboBox_Type.TabIndex = 5;
            // 
            // UI_Label_Type
            // 
            UI_Label_Type.AutoSize = true;
            UI_Label_Type.Location = new System.Drawing.Point(42, 80);
            UI_Label_Type.Name = "UI_Label_Type";
            UI_Label_Type.Size = new System.Drawing.Size(34, 15);
            UI_Label_Type.TabIndex = 4;
            UI_Label_Type.Text = "Type:";
            // 
            // UI_Label_Source
            // 
            UI_Label_Source.AutoSize = true;
            UI_Label_Source.Location = new System.Drawing.Point(30, 23);
            UI_Label_Source.Name = "UI_Label_Source";
            UI_Label_Source.Size = new System.Drawing.Size(46, 15);
            UI_Label_Source.TabIndex = 3;
            UI_Label_Source.Text = "Source:";
            // 
            // UI_Label_Destination
            // 
            UI_Label_Destination.AutoSize = true;
            UI_Label_Destination.Location = new System.Drawing.Point(6, 51);
            UI_Label_Destination.Name = "UI_Label_Destination";
            UI_Label_Destination.Size = new System.Drawing.Size(70, 15);
            UI_Label_Destination.TabIndex = 2;
            UI_Label_Destination.Text = "Destination:";
            // 
            // UI_TextBox_Destination
            // 
            this.UI_TextBox_Destination.Location = new System.Drawing.Point(82, 48);
            this.UI_TextBox_Destination.Name = "UI_TextBox_Destination";
            this.UI_TextBox_Destination.Size = new System.Drawing.Size(348, 23);
            this.UI_TextBox_Destination.TabIndex = 1;
            // 
            // UI_TextBox_Source
            // 
            this.UI_TextBox_Source.Location = new System.Drawing.Point(82, 20);
            this.UI_TextBox_Source.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.UI_TextBox_Source.Name = "UI_TextBox_Source";
            this.UI_TextBox_Source.Size = new System.Drawing.Size(348, 23);
            this.UI_TextBox_Source.TabIndex = 0;
            // 
            // UI_MenuStrip
            // 
            this.UI_MenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.UI_MenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.UI_MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            UI_MenuStrip_File,
            UI_MenuStrip_Help});
            this.UI_MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.UI_MenuStrip.Name = "UI_MenuStrip";
            this.UI_MenuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.UI_MenuStrip.Size = new System.Drawing.Size(515, 24);
            this.UI_MenuStrip.TabIndex = 0;
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 185);
            this.Controls.Add(UI_GroupBox_Backup);
            this.Controls.Add(UI_StatusStrip);
            this.Controls.Add(this.UI_MenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.UI_MenuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximumSize = new System.Drawing.Size(697, 500);
            this.MinimumSize = new System.Drawing.Size(531, 202);
            this.Name = "UI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backup Assistant";
            UI_StatusStrip.ResumeLayout(false);
            UI_StatusStrip.PerformLayout();
            UI_GroupBox_Backup.ResumeLayout(false);
            UI_GroupBox_Backup.PerformLayout();
            this.UI_MenuStrip.ResumeLayout(false);
            this.UI_MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip UI_MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem UI_MenuStrip_Exit;
        private System.Windows.Forms.ToolStripMenuItem UI_MenuStrip_About;
        private System.Windows.Forms.ToolStripStatusLabel UI_ToolStripStatusLabel_Progress;
        private System.Windows.Forms.ToolStripProgressBar UI_ToolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel UI_ToolStripStatus;
        private System.Windows.Forms.TextBox UI_TextBox_Source;
        private System.Windows.Forms.TextBox UI_TextBox_Destination;
        private System.Windows.Forms.ComboBox UI_ComboBox_Type;
        private System.Windows.Forms.Button UI_Button_Filter;
        private System.Windows.Forms.Button UI_Button_Source;
        private System.Windows.Forms.Button UI_Button_Destination;
        private System.Windows.Forms.Button UI_Button_Backup;
        private System.Windows.Forms.Button UI_Button_Cancel;
    }
}

