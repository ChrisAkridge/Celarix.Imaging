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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			TSBOpenFiles = new System.Windows.Forms.ToolStripButton();
			TSBOpenPicture = new System.Windows.Forms.ToolStripButton();
			TSBOpenFolder = new System.Windows.Forms.ToolStripButton();
			TSBOpenRaw = new System.Windows.Forms.ToolStripButton();
			TSBSaveAs = new System.Windows.Forms.ToolStripButton();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			TSBRefresh = new System.Windows.Forms.ToolStripButton();
			TSBCancel = new System.Windows.Forms.ToolStripButton();
			TSBLargeFileProcessor = new System.Windows.Forms.ToolStripButton();
			TSBSort = new System.Windows.Forms.ToolStripButton();
			TSBUnique = new System.Windows.Forms.ToolStripButton();
			ProgressBar = new System.Windows.Forms.ProgressBar();
			StaticLabelBitDepth = new System.Windows.Forms.Label();
			ComboBitDepths = new System.Windows.Forms.ComboBox();
			StaticLabelColorMode = new System.Windows.Forms.Label();
			RadioGrayscale = new System.Windows.Forms.RadioButton();
			RadioRGB = new System.Windows.Forms.RadioButton();
			RadioARGB = new System.Windows.Forms.RadioButton();
			RadioPaletted = new System.Windows.Forms.RadioButton();
			ButtonPalette = new System.Windows.Forms.Button();
			OpenFile = new System.Windows.Forms.OpenFileDialog();
			SaveFile = new System.Windows.Forms.SaveFileDialog();
			Panel = new System.Windows.Forms.Panel();
			PictureBox = new System.Windows.Forms.PictureBox();
			FolderSelector = new System.Windows.Forms.FolderBrowserDialog();
			OpenPicture = new System.Windows.Forms.OpenFileDialog();
			LabelAddress = new System.Windows.Forms.Label();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			TSBChromaPlayground = new System.Windows.Forms.ToolStripButton();
			toolStrip1.SuspendLayout();
			Panel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PictureBox).BeginInit();
			SuspendLayout();
			// 
			// toolStrip1
			// 
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TSBOpenFiles, TSBOpenPicture, TSBOpenFolder, TSBOpenRaw, TSBSaveAs, toolStripSeparator1, TSBRefresh, TSBCancel, TSBLargeFileProcessor, TSBSort, TSBUnique, toolStripSeparator2, TSBChromaPlayground });
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new System.Drawing.Size(1061, 25);
			toolStrip1.TabIndex = 0;
			toolStrip1.Text = "toolStrip1";
			// 
			// TSBOpenFiles
			// 
			TSBOpenFiles.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			TSBOpenFiles.Image = (System.Drawing.Image)resources.GetObject("TSBOpenFiles.Image");
			TSBOpenFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBOpenFiles.Name = "TSBOpenFiles";
			TSBOpenFiles.Size = new System.Drawing.Size(99, 22);
			TSBOpenFiles.Text = "&Open File(s)...";
			TSBOpenFiles.Click += TSBOpenFiles_Click;
			// 
			// TSBOpenPicture
			// 
			TSBOpenPicture.Image = (System.Drawing.Image)resources.GetObject("TSBOpenPicture.Image");
			TSBOpenPicture.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBOpenPicture.Name = "TSBOpenPicture";
			TSBOpenPicture.Size = new System.Drawing.Size(105, 22);
			TSBOpenPicture.Text = "Open &Picture...";
			TSBOpenPicture.Click += TSBOpenPicture_Click;
			// 
			// TSBOpenFolder
			// 
			TSBOpenFolder.Image = (System.Drawing.Image)resources.GetObject("TSBOpenFolder.Image");
			TSBOpenFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBOpenFolder.Name = "TSBOpenFolder";
			TSBOpenFolder.Size = new System.Drawing.Size(101, 22);
			TSBOpenFolder.Text = "&Open Folder...";
			TSBOpenFolder.Click += TSBOpenFolder_Click;
			// 
			// TSBOpenRaw
			// 
			TSBOpenRaw.Image = (System.Drawing.Image)resources.GetObject("TSBOpenRaw.Image");
			TSBOpenRaw.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBOpenRaw.Name = "TSBOpenRaw";
			TSBOpenRaw.Size = new System.Drawing.Size(81, 22);
			TSBOpenRaw.Text = "Open &Raw";
			TSBOpenRaw.Click += TSBOpenRaw_Click;
			// 
			// TSBSaveAs
			// 
			TSBSaveAs.Image = (System.Drawing.Image)resources.GetObject("TSBSaveAs.Image");
			TSBSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBSaveAs.Name = "TSBSaveAs";
			TSBSaveAs.Size = new System.Drawing.Size(76, 22);
			TSBSaveAs.Text = "Save &As...";
			TSBSaveAs.Click += TSBSaveAs_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// TSBRefresh
			// 
			TSBRefresh.Image = (System.Drawing.Image)resources.GetObject("TSBRefresh.Image");
			TSBRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBRefresh.Name = "TSBRefresh";
			TSBRefresh.Size = new System.Drawing.Size(66, 22);
			TSBRefresh.Text = "&Refresh";
			TSBRefresh.Click += TSBRefresh_Click;
			// 
			// TSBCancel
			// 
			TSBCancel.Image = (System.Drawing.Image)resources.GetObject("TSBCancel.Image");
			TSBCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBCancel.Name = "TSBCancel";
			TSBCancel.Size = new System.Drawing.Size(63, 22);
			TSBCancel.Text = "&Cancel";
			TSBCancel.Click += TSBCancel_Click;
			// 
			// TSBLargeFileProcessor
			// 
			TSBLargeFileProcessor.Image = (System.Drawing.Image)resources.GetObject("TSBLargeFileProcessor.Image");
			TSBLargeFileProcessor.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBLargeFileProcessor.Name = "TSBLargeFileProcessor";
			TSBLargeFileProcessor.Size = new System.Drawing.Size(140, 22);
			TSBLargeFileProcessor.Text = "Large File Processor...";
			TSBLargeFileProcessor.Click += TSBLargeFileProcessor_Click;
			// 
			// TSBSort
			// 
			TSBSort.Image = (System.Drawing.Image)resources.GetObject("TSBSort.Image");
			TSBSort.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBSort.Name = "TSBSort";
			TSBSort.Size = new System.Drawing.Size(48, 22);
			TSBSort.Text = "&Sort";
			TSBSort.Click += TSBSort_Click;
			// 
			// TSBUnique
			// 
			TSBUnique.Image = (System.Drawing.Image)resources.GetObject("TSBUnique.Image");
			TSBUnique.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBUnique.Name = "TSBUnique";
			TSBUnique.Size = new System.Drawing.Size(102, 22);
			TSBUnique.Text = "Unique Colors";
			TSBUnique.Click += TSBUnique_Click;
			// 
			// ProgressBar
			// 
			ProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			ProgressBar.Location = new System.Drawing.Point(13, 454);
			ProgressBar.Name = "ProgressBar";
			ProgressBar.Size = new System.Drawing.Size(1036, 12);
			ProgressBar.TabIndex = 2;
			// 
			// StaticLabelBitDepth
			// 
			StaticLabelBitDepth.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			StaticLabelBitDepth.AutoSize = true;
			StaticLabelBitDepth.Location = new System.Drawing.Point(13, 476);
			StaticLabelBitDepth.Name = "StaticLabelBitDepth";
			StaticLabelBitDepth.Size = new System.Drawing.Size(58, 13);
			StaticLabelBitDepth.TabIndex = 3;
			StaticLabelBitDepth.Text = "Bit Depth:";
			// 
			// ComboBitDepths
			// 
			ComboBitDepths.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			ComboBitDepths.FormattingEnabled = true;
			ComboBitDepths.Items.AddRange(new object[] { "1bpp", "2bpp", "4bpp", "8bpp", "16bpp", "24bpp", "32bpp" });
			ComboBitDepths.Location = new System.Drawing.Point(69, 473);
			ComboBitDepths.Name = "ComboBitDepths";
			ComboBitDepths.Size = new System.Drawing.Size(112, 21);
			ComboBitDepths.TabIndex = 4;
			ComboBitDepths.SelectedIndexChanged += ComboBitDepths_SelectedIndexChanged;
			// 
			// StaticLabelColorMode
			// 
			StaticLabelColorMode.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			StaticLabelColorMode.AutoSize = true;
			StaticLabelColorMode.Location = new System.Drawing.Point(624, 476);
			StaticLabelColorMode.Name = "StaticLabelColorMode";
			StaticLabelColorMode.Size = new System.Drawing.Size(71, 13);
			StaticLabelColorMode.TabIndex = 5;
			StaticLabelColorMode.Text = "Color Mode:";
			// 
			// RadioGrayscale
			// 
			RadioGrayscale.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			RadioGrayscale.AutoSize = true;
			RadioGrayscale.Location = new System.Drawing.Point(701, 474);
			RadioGrayscale.Name = "RadioGrayscale";
			RadioGrayscale.Size = new System.Drawing.Size(73, 17);
			RadioGrayscale.TabIndex = 6;
			RadioGrayscale.TabStop = true;
			RadioGrayscale.Text = "&Grayscale";
			RadioGrayscale.UseVisualStyleBackColor = true;
			RadioGrayscale.CheckedChanged += RadioGrayscale_CheckedChanged;
			// 
			// RadioRGB
			// 
			RadioRGB.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			RadioRGB.AutoSize = true;
			RadioRGB.Location = new System.Drawing.Point(781, 474);
			RadioRGB.Name = "RadioRGB";
			RadioRGB.Size = new System.Drawing.Size(46, 17);
			RadioRGB.TabIndex = 7;
			RadioRGB.TabStop = true;
			RadioRGB.Text = "&RGB";
			RadioRGB.UseVisualStyleBackColor = true;
			RadioRGB.CheckedChanged += RadioRGB_CheckedChanged;
			// 
			// RadioARGB
			// 
			RadioARGB.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			RadioARGB.AutoSize = true;
			RadioARGB.Location = new System.Drawing.Point(834, 474);
			RadioARGB.Name = "RadioARGB";
			RadioARGB.Size = new System.Drawing.Size(53, 17);
			RadioARGB.TabIndex = 8;
			RadioARGB.TabStop = true;
			RadioARGB.Text = "ARG&B";
			RadioARGB.UseVisualStyleBackColor = true;
			RadioARGB.CheckedChanged += RadioARGB_CheckedChanged;
			// 
			// RadioPaletted
			// 
			RadioPaletted.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			RadioPaletted.AutoSize = true;
			RadioPaletted.Location = new System.Drawing.Point(894, 474);
			RadioPaletted.Name = "RadioPaletted";
			RadioPaletted.Size = new System.Drawing.Size(67, 17);
			RadioPaletted.TabIndex = 9;
			RadioPaletted.TabStop = true;
			RadioPaletted.Text = "&Paletted";
			RadioPaletted.UseVisualStyleBackColor = true;
			RadioPaletted.Visible = false;
			RadioPaletted.CheckedChanged += RadioPaletted_CheckedChanged;
			// 
			// ButtonPalette
			// 
			ButtonPalette.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonPalette.Location = new System.Drawing.Point(967, 471);
			ButtonPalette.Name = "ButtonPalette";
			ButtonPalette.Size = new System.Drawing.Size(82, 23);
			ButtonPalette.TabIndex = 10;
			ButtonPalette.Text = "&Palette...";
			ButtonPalette.UseVisualStyleBackColor = true;
			ButtonPalette.Visible = false;
			// 
			// OpenFile
			// 
			OpenFile.Filter = "All files|*.*";
			OpenFile.Multiselect = true;
			OpenFile.SupportMultiDottedExtensions = true;
			OpenFile.Title = "Open Files";
			// 
			// SaveFile
			// 
			SaveFile.DefaultExt = "png";
			SaveFile.Filter = "PNG Image|*.png|JPEG Image|*.jpg|GIF Image|*.gif|Bitmap|*.bmp|Raw File|*.raw";
			SaveFile.Title = "Save File As";
			// 
			// Panel
			// 
			Panel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			Panel.AutoScroll = true;
			Panel.Controls.Add(PictureBox);
			Panel.Location = new System.Drawing.Point(13, 29);
			Panel.Name = "Panel";
			Panel.Size = new System.Drawing.Size(1036, 419);
			Panel.TabIndex = 11;
			Panel.MouseEnter += Panel_MouseEnter;
			// 
			// PictureBox
			// 
			PictureBox.BackColor = System.Drawing.SystemColors.Control;
			PictureBox.Location = new System.Drawing.Point(0, 0);
			PictureBox.Name = "PictureBox";
			PictureBox.Size = new System.Drawing.Size(679, 413);
			PictureBox.TabIndex = 0;
			PictureBox.TabStop = false;
			PictureBox.MouseMove += Image_MouseMove;
			// 
			// FolderSelector
			// 
			FolderSelector.Description = "Select a folder to load files from. WARNING: Operates recursively. I wouldn't suggest selecting a folder with thousands of files.";
			// 
			// OpenPicture
			// 
			OpenPicture.FileName = "openFileDialog1";
			OpenPicture.Filter = "JPEG Image|*.jpg|GIF Image|*.gif|PNG Image|*.png|Bitmap|*.bmp|All files|*.*";
			OpenPicture.Title = "Open Picture";
			// 
			// LabelAddress
			// 
			LabelAddress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			LabelAddress.AutoSize = true;
			LabelAddress.Location = new System.Drawing.Point(187, 476);
			LabelAddress.Name = "LabelAddress";
			LabelAddress.Size = new System.Drawing.Size(75, 13);
			LabelAddress.TabIndex = 12;
			LabelAddress.Text = "0x00000000:0";
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// TSBChromaPlayground
			// 
			TSBChromaPlayground.Image = (System.Drawing.Image)resources.GetObject("TSBChromaPlayground.Image");
			TSBChromaPlayground.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBChromaPlayground.Name = "TSBChromaPlayground";
			TSBChromaPlayground.Size = new System.Drawing.Size(134, 22);
			TSBChromaPlayground.Text = "Chroma Playground";
			TSBChromaPlayground.Click += TSBChromaPlayground_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1061, 501);
			Controls.Add(LabelAddress);
			Controls.Add(Panel);
			Controls.Add(ButtonPalette);
			Controls.Add(RadioPaletted);
			Controls.Add(RadioARGB);
			Controls.Add(RadioRGB);
			Controls.Add(RadioGrayscale);
			Controls.Add(StaticLabelColorMode);
			Controls.Add(ComboBitDepths);
			Controls.Add(StaticLabelBitDepth);
			Controls.Add(ProgressBar);
			Controls.Add(toolStrip1);
			Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			MinimumSize = new System.Drawing.Size(720, 540);
			Name = "MainForm";
			Text = "ByteView";
			Load += MainForm_Load;
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			Panel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PictureBox).EndInit();
			ResumeLayout(false);
			PerformLayout();
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
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton TSBChromaPlayground;
	}
}