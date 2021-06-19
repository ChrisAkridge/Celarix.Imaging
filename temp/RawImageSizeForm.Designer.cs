namespace Celarix.Imaging.ByteView
{
    partial class RawImageSizeForm
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
            this.StaticLabelWidth = new System.Windows.Forms.Label();
            this.TextBoxWidth = new System.Windows.Forms.TextBox();
            this.TextBoxHeight = new System.Windows.Forms.TextBox();
            this.StaticLabelHeight = new System.Windows.Forms.Label();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.StaticLabelTip = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StaticLabelWidth
            // 
            this.StaticLabelWidth.AutoSize = true;
            this.StaticLabelWidth.Location = new System.Drawing.Point(13, 13);
            this.StaticLabelWidth.Name = "StaticLabelWidth";
            this.StaticLabelWidth.Size = new System.Drawing.Size(42, 13);
            this.StaticLabelWidth.TabIndex = 0;
            this.StaticLabelWidth.Text = "Width:";
            // 
            // TextBoxWidth
            // 
            this.TextBoxWidth.Location = new System.Drawing.Point(61, 10);
            this.TextBoxWidth.Name = "TextBoxWidth";
            this.TextBoxWidth.Size = new System.Drawing.Size(55, 22);
            this.TextBoxWidth.TabIndex = 1;
            this.TextBoxWidth.Text = "0";
            // 
            // TextBoxHeight
            // 
            this.TextBoxHeight.Location = new System.Drawing.Point(170, 10);
            this.TextBoxHeight.Name = "TextBoxHeight";
            this.TextBoxHeight.Size = new System.Drawing.Size(55, 22);
            this.TextBoxHeight.TabIndex = 3;
            this.TextBoxHeight.Text = "0";
            // 
            // StaticLabelHeight
            // 
            this.StaticLabelHeight.AutoSize = true;
            this.StaticLabelHeight.Location = new System.Drawing.Point(122, 13);
            this.StaticLabelHeight.Name = "StaticLabelHeight";
            this.StaticLabelHeight.Size = new System.Drawing.Size(45, 13);
            this.StaticLabelHeight.TabIndex = 2;
            this.StaticLabelHeight.Text = "Height:";
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(231, 8);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 4;
            this.ButtonOK.Text = "&OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            this.ButtonOK.Click += new System.EventHandler(this.ButtonOK_Click);
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ButtonCancel.Location = new System.Drawing.Point(312, 8);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 5;
            this.ButtonCancel.Text = "&Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // StaticLabelTip
            // 
            this.StaticLabelTip.AutoSize = true;
            this.StaticLabelTip.Location = new System.Drawing.Point(13, 36);
            this.StaticLabelTip.Name = "StaticLabelTip";
            this.StaticLabelTip.Size = new System.Drawing.Size(212, 13);
            this.StaticLabelTip.TabIndex = 6;
            this.StaticLabelTip.Text = "Tip: Put the image\'s size in the filename.";
            // 
            // RawImageSizeForm
            // 
            this.AcceptButton = this.ButtonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ButtonCancel;
            this.ClientSize = new System.Drawing.Size(392, 61);
            this.Controls.Add(this.StaticLabelTip);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.TextBoxHeight);
            this.Controls.Add(this.StaticLabelHeight);
            this.Controls.Add(this.TextBoxWidth);
            this.Controls.Add(this.StaticLabelWidth);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "RawImageSizeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Select Size for Raw Image";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StaticLabelWidth;
        private System.Windows.Forms.TextBox TextBoxWidth;
        private System.Windows.Forms.TextBox TextBoxHeight;
        private System.Windows.Forms.Label StaticLabelHeight;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonCancel;
        private System.Windows.Forms.Label StaticLabelTip;
    }
}