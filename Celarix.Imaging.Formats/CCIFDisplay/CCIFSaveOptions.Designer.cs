namespace CCIFDisplay
{
    partial class CCIFSaveOptions
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
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.GroupCompressionMode = new System.Windows.Forms.GroupBox();
            this.RadioNoCompression = new System.Windows.Forms.RadioButton();
            this.RadioZlibCompression = new System.Windows.Forms.RadioButton();
            this.GroupCompressionMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonOK
            // 
            this.ButtonOK.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonOK.Location = new System.Drawing.Point(122, 119);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(91, 37);
            this.ButtonOK.TabIndex = 0;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonCancel.Location = new System.Drawing.Point(25, 119);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(91, 37);
            this.ButtonCancel.TabIndex = 1;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            this.ButtonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // GroupCompressionMode
            // 
            this.GroupCompressionMode.Controls.Add(this.RadioZlibCompression);
            this.GroupCompressionMode.Controls.Add(this.RadioNoCompression);
            this.GroupCompressionMode.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupCompressionMode.Location = new System.Drawing.Point(13, 13);
            this.GroupCompressionMode.Name = "GroupCompressionMode";
            this.GroupCompressionMode.Size = new System.Drawing.Size(200, 100);
            this.GroupCompressionMode.TabIndex = 2;
            this.GroupCompressionMode.TabStop = false;
            this.GroupCompressionMode.Text = "Compression Mode";
            // 
            // RadioNoCompression
            // 
            this.RadioNoCompression.AutoSize = true;
            this.RadioNoCompression.Location = new System.Drawing.Point(7, 29);
            this.RadioNoCompression.Name = "RadioNoCompression";
            this.RadioNoCompression.Size = new System.Drawing.Size(152, 25);
            this.RadioNoCompression.TabIndex = 0;
            this.RadioNoCompression.TabStop = true;
            this.RadioNoCompression.Text = "No Compression";
            this.RadioNoCompression.UseVisualStyleBackColor = true;
            // 
            // RadioZlibCompression
            // 
            this.RadioZlibCompression.AutoSize = true;
            this.RadioZlibCompression.Checked = true;
            this.RadioZlibCompression.Location = new System.Drawing.Point(7, 61);
            this.RadioZlibCompression.Name = "RadioZlibCompression";
            this.RadioZlibCompression.Size = new System.Drawing.Size(161, 25);
            this.RadioZlibCompression.TabIndex = 1;
            this.RadioZlibCompression.TabStop = true;
            this.RadioZlibCompression.Text = "ZLib Compression";
            this.RadioZlibCompression.UseVisualStyleBackColor = true;
            // 
            // CCIFSaveOptions
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(231, 176);
            this.Controls.Add(this.GroupCompressionMode);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CCIFSaveOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CCIF Save Options";
            this.GroupCompressionMode.ResumeLayout(false);
            this.GroupCompressionMode.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.GroupBox GroupCompressionMode;
        private System.Windows.Forms.RadioButton RadioZlibCompression;
        private System.Windows.Forms.RadioButton RadioNoCompression;
    }
}