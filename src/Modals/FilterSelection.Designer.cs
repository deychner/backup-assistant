
namespace BackupAssistant.Modals
{
    partial class FilterSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterSelection));
            this.FilterSelection_CheckedListBox_Filters = new System.Windows.Forms.CheckedListBox();
            this.FilterSelection_Button_Cancel = new System.Windows.Forms.Button();
            this.FilterSelection_Button_Apply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FilterSelection_CheckedListBox_Filters
            // 
            this.FilterSelection_CheckedListBox_Filters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterSelection_CheckedListBox_Filters.FormattingEnabled = true;
            this.FilterSelection_CheckedListBox_Filters.Location = new System.Drawing.Point(12, 12);
            this.FilterSelection_CheckedListBox_Filters.Name = "FilterSelection_CheckedListBox_Filters";
            this.FilterSelection_CheckedListBox_Filters.Size = new System.Drawing.Size(260, 202);
            this.FilterSelection_CheckedListBox_Filters.TabIndex = 0;
            // 
            // FilterSelection_Button_Cancel
            // 
            this.FilterSelection_Button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterSelection_Button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.FilterSelection_Button_Cancel.Location = new System.Drawing.Point(197, 226);
            this.FilterSelection_Button_Cancel.Name = "FilterSelection_Button_Cancel";
            this.FilterSelection_Button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.FilterSelection_Button_Cancel.TabIndex = 1;
            this.FilterSelection_Button_Cancel.Text = "Cancel";
            this.FilterSelection_Button_Cancel.UseVisualStyleBackColor = true;
            // 
            // FilterSelection_Button_Apply
            // 
            this.FilterSelection_Button_Apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterSelection_Button_Apply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.FilterSelection_Button_Apply.Location = new System.Drawing.Point(116, 226);
            this.FilterSelection_Button_Apply.Name = "FilterSelection_Button_Apply";
            this.FilterSelection_Button_Apply.Size = new System.Drawing.Size(75, 23);
            this.FilterSelection_Button_Apply.TabIndex = 2;
            this.FilterSelection_Button_Apply.Text = "Apply";
            this.FilterSelection_Button_Apply.UseVisualStyleBackColor = true;
            // 
            // FilterSelection
            // 
            this.AcceptButton = this.FilterSelection_Button_Apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.FilterSelection_Button_Cancel;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.FilterSelection_Button_Apply);
            this.Controls.Add(this.FilterSelection_Button_Cancel);
            this.Controls.Add(this.FilterSelection_CheckedListBox_Filters);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(300, 100);
            this.Name = "FilterSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filter Selection";
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.CheckedListBox FilterSelection_CheckedListBox_Filters;
        private System.Windows.Forms.Button FilterSelection_Button_Cancel;
        private System.Windows.Forms.Button FilterSelection_Button_Apply;
    }
}