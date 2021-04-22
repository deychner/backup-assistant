
namespace BackupAssistant.Modals
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.About_Label_ApplicationName = new System.Windows.Forms.Label();
            this.About_Label_Version = new System.Windows.Forms.Label();
            this.About_Button_OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // About_Label_ApplicationName
            // 
            this.About_Label_ApplicationName.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.About_Label_ApplicationName.Location = new System.Drawing.Point(12, 9);
            this.About_Label_ApplicationName.Name = "About_Label_ApplicationName";
            this.About_Label_ApplicationName.Size = new System.Drawing.Size(303, 51);
            this.About_Label_ApplicationName.TabIndex = 0;
            this.About_Label_ApplicationName.Text = "Name";
            this.About_Label_ApplicationName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // About_Label_Version
            // 
            this.About_Label_Version.Location = new System.Drawing.Point(12, 65);
            this.About_Label_Version.Name = "About_Label_Version";
            this.About_Label_Version.Size = new System.Drawing.Size(303, 13);
            this.About_Label_Version.TabIndex = 1;
            this.About_Label_Version.Text = "Version";
            this.About_Label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // About_Button_OK
            // 
            this.About_Button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.About_Button_OK.Location = new System.Drawing.Point(124, 96);
            this.About_Button_OK.Name = "About_Button_OK";
            this.About_Button_OK.Size = new System.Drawing.Size(75, 23);
            this.About_Button_OK.TabIndex = 2;
            this.About_Button_OK.Text = "OK";
            this.About_Button_OK.UseVisualStyleBackColor = true;
            this.About_Button_OK.Click += new System.EventHandler(this.About_Button_OK_Click);
            // 
            // About
            // 
            this.AcceptButton = this.About_Button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(327, 131);
            this.Controls.Add(this.About_Button_OK);
            this.Controls.Add(this.About_Label_Version);
            this.Controls.Add(this.About_Label_ApplicationName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label About_Label_ApplicationName;
        private System.Windows.Forms.Label About_Label_Version;
        private System.Windows.Forms.Button About_Button_OK;
    }
}