namespace Celarix.Imaging.ByteView
{
	/// <summary>
	/// The main form for ByteView.
	/// </summary>
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
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.TSBOpenFiles = new System.Windows.Forms.ToolStripButton();
			this.TSBOpenPicture = new System.Windows.Forms.ToolStripButton();
			this.TSBOpenFolder = new System.Windows.Forms.ToolStripButton();
			this.TSBOpenRaw = new System.Windows.Forms.ToolStripButton();
			this.TSBSaveAs = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.TSBRefresh = new System.Windows.Forms.ToolStripButton();
			this.TSBCancel = new System.Windows.Forms.ToolStripButton();
			this.TSBLargeFileProcessor = new System.Windows.Forms.ToolStripButton();
			this.TSBSort = new System.Windows.Forms.ToolStripButton();
			this.TSBUnique = new System.Windows.Forms.ToolStripButton();
			this.ProgressBar = new System.Windows.Forms.ProgressBar();
			this.StaticLabelBitDepth = new System.Windows.Forms.Label();
			this.Worker = new System.ComponentModel.BackgroundWorker();
			this.ComboBitDepths = new System.Windows.Forms.ComboBox();
			this.StaticLabelColorMode = new System.Windows.Forms.Label();
			this.RadioGrayscale = new System.Windows.Forms.RadioButton();
			this.RadioRGB = new System.Windows.Forms.RadioButton();
			this.RadioARGB = new System.Windows.Forms.RadioButton();
			this.RadioPaletted = new System.Windows.Forms.RadioButton();
			this.ButtonPalette = new System.Windows.Forms.Button();
			this.OpenFile = new System.Windows.Forms.OpenFileDialog();
			this.SaveFile = new System.Windows.Forms.SaveFileDialog();
			this.Panel = new System.Windows.Forms.Panel();
			this.PictureBox = new System.Windows.Forms.PictureBox();
			this.FolderSelector = new System.Windows.Forms.FolderBrowserDialog();
			this.OpenPicture = new System.Windows.Forms.OpenFileDialog();
			this.LabelAddress = new System.Windows.Forms.Label();
			this.toolStrip1.SuspendLayout();
			this.Panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSBOpenFiles,
            this.TSBOpenPicture,
            this.TSBOpenFolder,
            this.TSBOpenRaw,
            this.TSBSaveAs,
            this.toolStripSeparator1,
            this.TSBRefresh,
            this.TSBCancel,
            this.TSBLargeFileProcessor,
            this.TSBSort,
            this.TSBUnique});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(704, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// TSBOpenFiles
			// 
			this.TSBOpenFiles.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TSBOpenFiles.Image = ((System.Drawing.Image)(resources.GetObject("TSBOpenFiles.Image")));
			this.TSBOpenFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBOpenFiles.Name = "TSBOpenFiles";
			this.TSBOpenFiles.Size = new System.Drawing.Size(99, 22);
			this.TSBOpenFiles.Text = "&Open File(s)...";
			this.TSBOpenFiles.Click += new System.EventHandler(this.TSBOpenFiles_Click);
			// 
			// TSBOpenPicture
			// 
			this.TSBOpenPicture.Image = ((System.Drawing.Image)(resources.GetObject("TSBOpenPicture.Image")));
			this.TSBOpenPicture.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBOpenPicture.Name = "TSBOpenPicture";
			this.TSBOpenPicture.Size = new System.Drawing.Size(105, 22);
			this.TSBOpenPicture.Text = "Open &Picture...";
			this.TSBOpenPicture.Click += new System.EventHandler(this.TSBOpenPicture_Click);
			// 
			// TSBOpenFolder
			// 
			this.TSBOpenFolder.Image = ((System.Drawing.Image)(resources.GetObject("TSBOpenFolder.Image")));
			this.TSBOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBOpenFolder.Name = "TSBOpenFolder";
			this.TSBOpenFolder.Size = new System.Drawing.Size(101, 22);
			this.TSBOpenFolder.Text = "&Open Folder...";
			this.TSBOpenFolder.Click += new System.EventHandler(this.TSBOpenFolder_Click);
			// 
			// TSBOpenRaw
			// 
			this.TSBOpenRaw.Image = ((System.Drawing.Image)(resources.GetObject("TSBOpenRaw.Image")));
			this.TSBOpenRaw.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBOpenRaw.Name = "TSBOpenRaw";
			this.TSBOpenRaw.Size = new System.Drawing.Size(81, 22);
			this.TSBOpenRaw.Text = "Open &Raw";
			this.TSBOpenRaw.Click += new System.EventHandler(this.TSBOpenRaw_Click);
			// 
			// TSBSaveAs
			// 
			this.TSBSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("TSBSaveAs.Image")));
			this.TSBSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBSaveAs.Name = "TSBSaveAs";
			this.TSBSaveAs.Size = new System.Drawing.Size(76, 22);
			this.TSBSaveAs.Text = "Save &As...";
			this.TSBSaveAs.Click += new System.EventHandler(this.TSBSaveAs_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// TSBRefresh
			// 
			this.TSBRefresh.Image = ((System.Drawing.Image)(resources.GetObject("TSBRefresh.Image")));
			this.TSBRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBRefresh.Name = "TSBRefresh";
			this.TSBRefresh.Size = new System.Drawing.Size(66, 22);
			this.TSBRefresh.Text = "&Refresh";
			this.TSBRefresh.Click += new System.EventHandler(this.TSBRefresh_Click);
			// 
			// TSBCancel
			// 
			this.TSBCancel.Image = ((System.Drawing.Image)(resources.GetObject("TSBCancel.Image")));
			this.TSBCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBCancel.Name = "TSBCancel";
			this.TSBCancel.Size = new System.Drawing.Size(63, 22);
			this.TSBCancel.Text = "&Cancel";
			this.TSBCancel.Click += new System.EventHandler(this.TSBCancel_Click);
			// 
			// TSBLargeFileProcessor
			// 
			this.TSBLargeFileProcessor.Image = ((System.Drawing.Image)(resources.GetObject("TSBLargeFileProcessor.Image")));
			this.TSBLargeFileProcessor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBLargeFileProcessor.Name = "TSBLargeFileProcessor";
			this.TSBLargeFileProcessor.Size = new System.Drawing.Size(140, 22);
			this.TSBLargeFileProcessor.Text = "Large File Processor...";
			this.TSBLargeFileProcessor.Click += new System.EventHandler(this.TSBLargeFileProcessor_Click);
			// 
			// TSBSort
			// 
			this.TSBSort.Image = ((System.Drawing.Image)(resources.GetObject("TSBSort.Image")));
			this.TSBSort.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBSort.Name = "TSBSort";
			this.TSBSort.Size = new System.Drawing.Size(48, 22);
			this.TSBSort.Text = "&Sort";
			this.TSBSort.Click += new System.EventHandler(this.TSBSort_Click);
			// 
			// TSBUnique
			// 
			this.TSBUnique.Image = ((System.Drawing.Image)(resources.GetObject("TSBUnique.Image")));
			this.TSBUnique.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.TSBUnique.Name = "TSBUnique";
			this.TSBUnique.Size = new System.Drawing.Size(102, 20);
			this.TSBUnique.Text = "Unique Colors";
			this.TSBUnique.Click += new System.EventHandler(this.TSBUnique_Click);
			// 
			// ProgressBar
			// 
			this.ProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ProgressBar.Location = new System.Drawing.Point(13, 454);
			this.ProgressBar.Name = "ProgressBar";
			this.ProgressBar.Size = new System.Drawing.Size(679, 12);
			this.ProgressBar.TabIndex = 2;
			// 
			// StaticLabelBitDepth
			// 
			this.StaticLabelBitDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.StaticLabelBitDepth.AutoSize = true;
			this.StaticLabelBitDepth.Location = new System.Drawing.Point(13, 476);
			this.StaticLabelBitDepth.Name = "StaticLabelBitDepth";
			this.StaticLabelBitDepth.Size = new System.Drawing.Size(59, 13);
			this.StaticLabelBitDepth.TabIndex = 3;
			this.StaticLabelBitDepth.Text = "Bit Depth:";
			// 
			// Worker
			// 
			this.Worker.WorkerReportsProgress = true;
			this.Worker.WorkerSupportsCancellation = true;
			this.Worker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Worker_DoWork);
			this.Worker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Worker_ProgressChanged);
			this.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Worker_RunWorkerCompleted);
			// 
			// ComboBitDepths
			// 
			this.ComboBitDepths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.ComboBitDepths.FormattingEnabled = true;
			this.ComboBitDepths.Items.AddRange(new object[] {
            "1bpp",
            "2bpp",
            "4bpp",
            "8bpp",
            "16bpp",
            "24bpp",
            "32bpp"});
			this.ComboBitDepths.Location = new System.Drawing.Point(69, 473);
			this.ComboBitDepths.Name = "ComboBitDepths";
			this.ComboBitDepths.Size = new System.Drawing.Size(112, 21);
			this.ComboBitDepths.TabIndex = 4;
			this.ComboBitDepths.SelectedIndexChanged += new System.EventHandler(this.ComboBitDepths_SelectedIndexChanged);
			// 
			// StaticLabelColorMode
			// 
			this.StaticLabelColorMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.StaticLabelColorMode.AutoSize = true;
			this.StaticLabelColorMode.Location = new System.Drawing.Point(267, 476);
			this.StaticLabelColorMode.Name = "StaticLabelColorMode";
			this.StaticLabelColorMode.Size = new System.Drawing.Size(71, 13);
			this.StaticLabelColorMode.TabIndex = 5;
			this.StaticLabelColorMode.Text = "Color Mode:";
			// 
			// RadioGrayscale
			// 
			this.RadioGrayscale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioGrayscale.AutoSize = true;
			this.RadioGrayscale.Location = new System.Drawing.Point(344, 474);
			this.RadioGrayscale.Name = "RadioGrayscale";
			this.RadioGrayscale.Size = new System.Drawing.Size(73, 17);
			this.RadioGrayscale.TabIndex = 6;
			this.RadioGrayscale.TabStop = true;
			this.RadioGrayscale.Text = "&Grayscale";
			this.RadioGrayscale.UseVisualStyleBackColor = true;
			this.RadioGrayscale.CheckedChanged += new System.EventHandler(this.RadioGrayscale_CheckedChanged);
			// 
			// RadioRGB
			// 
			this.RadioRGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioRGB.AutoSize = true;
			this.RadioRGB.Location = new System.Drawing.Point(423, 474);
			this.RadioRGB.Name = "RadioRGB";
			this.RadioRGB.Size = new System.Drawing.Size(47, 17);
			this.RadioRGB.TabIndex = 7;
			this.RadioRGB.TabStop = true;
			this.RadioRGB.Text = "&RGB";
			this.RadioRGB.UseVisualStyleBackColor = true;
			this.RadioRGB.CheckedChanged += new System.EventHandler(this.RadioRGB_CheckedChanged);
			// 
			// RadioARGB
			// 
			this.RadioARGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioARGB.AutoSize = true;
			this.RadioARGB.Location = new System.Drawing.Point(476, 474);
			this.RadioARGB.Name = "RadioARGB";
			this.RadioARGB.Size = new System.Drawing.Size(54, 17);
			this.RadioARGB.TabIndex = 8;
			this.RadioARGB.TabStop = true;
			this.RadioARGB.Text = "ARG&B";
			this.RadioARGB.UseVisualStyleBackColor = true;
			this.RadioARGB.CheckedChanged += new System.EventHandler(this.RadioARGB_CheckedChanged);
			// 
			// RadioPaletted
			// 
			this.RadioPaletted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.RadioPaletted.AutoSize = true;
			this.RadioPaletted.Location = new System.Drawing.Point(537, 474);
			this.RadioPaletted.Name = "RadioPaletted";
			this.RadioPaletted.Size = new System.Drawing.Size(67, 17);
			this.RadioPaletted.TabIndex = 9;
			this.RadioPaletted.TabStop = true;
			this.RadioPaletted.Text = "&Paletted";
			this.RadioPaletted.UseVisualStyleBackColor = true;
			this.RadioPaletted.Visible = false;
			this.RadioPaletted.CheckedChanged += new System.EventHandler(this.RadioPaletted_CheckedChanged);
			// 
			// ButtonPalette
			// 
			this.ButtonPalette.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.ButtonPalette.Location = new System.Drawing.Point(610, 471);
			this.ButtonPalette.Name = "ButtonPalette";
			this.ButtonPalette.Size = new System.Drawing.Size(82, 23);
			this.ButtonPalette.TabIndex = 10;
			this.ButtonPalette.Text = "&Palette...";
			this.ButtonPalette.UseVisualStyleBackColor = true;
			this.ButtonPalette.Visible = false;
			// 
			// OpenFile
			// 
			this.OpenFile.Filter = "All files|*.*";
			this.OpenFile.Multiselect = true;
			this.OpenFile.SupportMultiDottedExtensions = true;
			this.OpenFile.Title = "Open Files";
			// 
			// SaveFile
			// 
			this.SaveFile.DefaultExt = "png";
			this.SaveFile.Filter = "PNG Image|*.png|JPEG Image|*.jpg|GIF Image|*.gif|Bitmap|*.bmp|Raw File|*.raw";
			this.SaveFile.Title = "Save File As";
			// 
			// Panel
			// 
			this.Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Panel.AutoScroll = true;
			this.Panel.Controls.Add(this.PictureBox);
			this.Panel.Location = new System.Drawing.Point(13, 29);
			this.Panel.Name = "Panel";
			this.Panel.Size = new System.Drawing.Size(679, 419);
			this.Panel.TabIndex = 11;
			this.Panel.MouseEnter += new System.EventHandler(this.Panel_MouseEnter);
			// 
			// PictureBox
			// 
			this.PictureBox.BackColor = System.Drawing.SystemColors.Control;
			this.PictureBox.Location = new System.Drawing.Point(0, 0);
			this.PictureBox.Name = "PictureBox";
			this.PictureBox.Size = new System.Drawing.Size(679, 413);
			this.PictureBox.TabIndex = 0;
			this.PictureBox.TabStop = false;
			this.PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Image_MouseMove);
			// 
			// FolderSelector
			// 
			this.FolderSelector.Description = "Select a folder to load files from. WARNING: Operates recursively. I wouldn\'t sug" +
    "gest selecting a folder with thousands of files.";
			// 
			// OpenPicture
			// 
			this.OpenPicture.FileName = "openFileDialog1";
			this.OpenPicture.Filter = "JPEG Image|*.jpg|GIF Image|*.gif|PNG Image|*.png|Bitmap|*.bmp|All files|*.*";
			this.OpenPicture.Title = "Open Picture";
			// 
			// LabelAddress
			// 
			this.LabelAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.LabelAddress.AutoSize = true;
			this.LabelAddress.Location = new System.Drawing.Point(187, 476);
			this.LabelAddress.Name = "LabelAddress";
			this.LabelAddress.Size = new System.Drawing.Size(75, 13);
			this.LabelAddress.TabIndex = 12;
			this.LabelAddress.Text = "0x00000000:0";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(704, 501);
			this.Controls.Add(this.LabelAddress);
			this.Controls.Add(this.Panel);
			this.Controls.Add(this.ButtonPalette);
			this.Controls.Add(this.RadioPaletted);
			this.Controls.Add(this.RadioARGB);
			this.Controls.Add(this.RadioRGB);
			this.Controls.Add(this.RadioGrayscale);
			this.Controls.Add(this.StaticLabelColorMode);
			this.Controls.Add(this.ComboBitDepths);
			this.Controls.Add(this.StaticLabelBitDepth);
			this.Controls.Add(this.ProgressBar);
			this.Controls.Add(this.toolStrip1);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(720, 540);
			this.Name = "MainForm";
			this.Text = "ByteView";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.Panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton TSBOpenFiles;
        private System.Windows.Forms.ToolStripButton TSBOpenFolder;
        private System.Windows.Forms.ToolStripButton TSBSaveAs;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton TSBRefresh;
        private System.Windows.Forms.ToolStripButton TSBCancel;
        private System.Windows.Forms.Label StaticLabelBitDepth;
        private System.ComponentModel.BackgroundWorker Worker;
        private System.Windows.Forms.ComboBox ComboBitDepths;
        private System.Windows.Forms.Label StaticLabelColorMode;
        private System.Windows.Forms.RadioButton RadioGrayscale;
        private System.Windows.Forms.RadioButton RadioRGB;
        private System.Windows.Forms.RadioButton RadioARGB;
        private System.Windows.Forms.RadioButton RadioPaletted;
        private System.Windows.Forms.Button ButtonPalette;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.Panel Panel;
        private System.Windows.Forms.PictureBox PictureBox;
        private System.Windows.Forms.FolderBrowserDialog FolderSelector;
        private System.Windows.Forms.ToolStripButton TSBOpenPicture;
        private System.Windows.Forms.OpenFileDialog OpenPicture;
        private System.Windows.Forms.ToolStripButton TSBOpenRaw;
        private System.Windows.Forms.ToolStripButton TSBLargeFileProcessor;
		private System.Windows.Forms.ToolStripButton TSBSort;
		private System.Windows.Forms.ToolStripButton TSBUnique;
		private System.Windows.Forms.Label LabelAddress;
	}
}