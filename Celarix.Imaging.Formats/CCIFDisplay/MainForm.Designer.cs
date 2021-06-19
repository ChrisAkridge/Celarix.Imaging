namespace CCIFDisplay
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.TSBOpenImage = new System.Windows.Forms.ToolStripButton();
            this.TSBOpenCCIF = new System.Windows.Forms.ToolStripButton();
            this.TSBSaveAsCCIF = new System.Windows.Forms.ToolStripButton();
            this.Picture = new System.Windows.Forms.PictureBox();
            this.OFDOpenImage = new System.Windows.Forms.OpenFileDialog();
            this.OFDOpenCCIF = new System.Windows.Forms.OpenFileDialog();
            this.SFDSaveCCIF = new System.Windows.Forms.SaveFileDialog();
            this.ToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolStrip
            // 
            this.ToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBOpenImage,
            this.TSBOpenCCIF,
            this.TSBSaveAsCCIF});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(800, 32);
            this.ToolStrip.TabIndex = 0;
            this.ToolStrip.Text = "toolStrip1";
            // 
            // TSBOpenImage
            // 
            this.TSBOpenImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBOpenImage.Image = ((System.Drawing.Image)(resources.GetObject("TSBOpenImage.Image")));
            this.TSBOpenImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBOpenImage.Name = "TSBOpenImage";
            this.TSBOpenImage.Size = new System.Drawing.Size(127, 29);
            this.TSBOpenImage.Text = "Open Image...";
            this.TSBOpenImage.Click += new System.EventHandler(this.TSBOpenImage_Click);
            // 
            // TSBOpenCCIF
            // 
            this.TSBOpenCCIF.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBOpenCCIF.Image = ((System.Drawing.Image)(resources.GetObject("TSBOpenCCIF.Image")));
            this.TSBOpenCCIF.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBOpenCCIF.Name = "TSBOpenCCIF";
            this.TSBOpenCCIF.Size = new System.Drawing.Size(113, 29);
            this.TSBOpenCCIF.Text = "Open CCIF...";
            this.TSBOpenCCIF.Click += new System.EventHandler(this.TSBOpenCCIF_Click);
            // 
            // TSBSaveAsCCIF
            // 
            this.TSBSaveAsCCIF.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.TSBSaveAsCCIF.Image = ((System.Drawing.Image)(resources.GetObject("TSBSaveAsCCIF.Image")));
            this.TSBSaveAsCCIF.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBSaveAsCCIF.Name = "TSBSaveAsCCIF";
            this.TSBSaveAsCCIF.Size = new System.Drawing.Size(131, 29);
            this.TSBSaveAsCCIF.Text = "Save As CCIF...";
            this.TSBSaveAsCCIF.Click += new System.EventHandler(this.TSBSaveAsCCIF_Click);
            // 
            // Picture
            // 
            this.Picture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Picture.Location = new System.Drawing.Point(0, 32);
            this.Picture.Name = "Picture";
            this.Picture.Size = new System.Drawing.Size(800, 440);
            this.Picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Picture.TabIndex = 1;
            this.Picture.TabStop = false;
            // 
            // OFDOpenImage
            // 
            this.OFDOpenImage.Filter = "JPG Files|*.jpg|JPEG Files|*.jpeg|PNG Files|*.png";
            this.OFDOpenImage.Title = "CCIF Display";
            // 
            // OFDOpenCCIF
            // 
            this.OFDOpenCCIF.Filter = "CCIF File|*.ccif";
            this.OFDOpenCCIF.Title = "CCIF Display";
            // 
            // SFDSaveCCIF
            // 
            this.SFDSaveCCIF.Filter = "CCIF Files|*.ccif";
            this.SFDSaveCCIF.Title = "CCIF Display";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 472);
            this.Controls.Add(this.Picture);
            this.Controls.Add(this.ToolStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MainForm";
            this.Text = "CCIF Display";
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripButton TSBOpenImage;
        private System.Windows.Forms.ToolStripButton TSBOpenCCIF;
        private System.Windows.Forms.ToolStripButton TSBSaveAsCCIF;
        private System.Windows.Forms.PictureBox Picture;
        private System.Windows.Forms.OpenFileDialog OFDOpenImage;
        private System.Windows.Forms.OpenFileDialog OFDOpenCCIF;
        private System.Windows.Forms.SaveFileDialog SFDSaveCCIF;
    }
}

