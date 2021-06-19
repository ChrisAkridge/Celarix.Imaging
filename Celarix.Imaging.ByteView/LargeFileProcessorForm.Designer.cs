using System.Windows.Forms;

namespace Celarix.Imaging.ByteView
{
    partial class LargeFileProcessorForm
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
			this.StaticLabelFiles = new System.Windows.Forms.Label();
			this.ButtonAddFolder = new System.Windows.Forms.Button();
			this.ButtonAddFiles = new System.Windows.Forms.Button();
			this.ListBoxFiles = new System.Windows.Forms.ListBox();
			this.LabelFilesData = new System.Windows.Forms.Label();
			this.StaticLabelSeparatorA = new System.Windows.Forms.Label();
			this.StaticLabelBitDepth = new System.Windows.Forms.Label();
			this.ComboBoxBitDepths = new System.Windows.Forms.ComboBox();
			this.StaticLabelColorMode = new System.Windows.Forms.Label();
			this.RadioGrayscale = new System.Windows.Forms.RadioButton();
			this.RadioRGB = new System.Windows.Forms.RadioButton();
			this.RadioARGB = new System.Windows.Forms.RadioButton();
			this.RadioPaletted = new System.Windows.Forms.RadioButton();
			this.ButtonEditPalette = new System.Windows.Forms.Button();
			this.StaticLabelImageWidth = new System.Windows.Forms.Label();
			this.StaticLabelImageHeight = new System.Windows.Forms.Label();
			this.TextBoxImageWidth = new System.Windows.Forms.TextBox();
			this.TextBoxImageHeight = new System.Windows.Forms.TextBox();
			this.LabelImageData = new System.Windows.Forms.Label();
			this.LabelOutputFolder = new System.Windows.Forms.Label();
			this.TextOutputFolder = new System.Windows.Forms.TextBox();
			this.ButtonSelectOutputFolder = new System.Windows.Forms.Button();
			this.ButtonClose = new System.Windows.Forms.Button();
			this.ButtonStop = new System.Windows.Forms.Button();
			this.ButtonGenerate = new System.Windows.Forms.Button();
			this.StaticLabelSeparatorB = new System.Windows.Forms.Label();
			this.LabelStatus = new System.Windows.Forms.Label();
			this.Progress = new System.Windows.Forms.ProgressBar();
			this.OFDAddFile = new System.Windows.Forms.OpenFileDialog();
			this.FBDAddFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.TextBoxFileEntry = new System.Windows.Forms.TextBox();
			this.FBDOutputFolder = new System.Windows.Forms.FolderBrowserDialog();
			this.CheckDrawFileNames = new System.Windows.Forms.CheckBox();
            this.CheckDrawAsZoomableTiles = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// StaticLabelFiles
			// 
			this.StaticLabelFiles.AutoSize = true;
			this.StaticLabelFiles.Location = new System.Drawing.Point(13, 13);
			this.StaticLabelFiles.Name = "StaticLabelFiles";
			this.StaticLabelFiles.Size = new System.Drawing.Size(46, 23);
			this.StaticLabelFiles.TabIndex = 0;
			this.StaticLabelFiles.Text = "Files:";
			// 
			// ButtonAddFolder
			// 
			this.ButtonAddFolder.Location = new System.Drawing.Point(452, 8);
			this.ButtonAddFolder.Name = "ButtonAddFolder";
			this.ButtonAddFolder.Size = new System.Drawing.Size(84, 23);
			this.ButtonAddFolder.TabIndex = 2;
			this.ButtonAddFolder.Text = "Add F&older...";
			this.ButtonAddFolder.UseVisualStyleBackColor = true;
			this.ButtonAddFolder.Click += new System.EventHandler(this.ButtonAddFolder_Click);
			// 
			// ButtonAddFiles
			// 
			this.ButtonAddFiles.Location = new System.Drawing.Point(371, 8);
			this.ButtonAddFiles.Name = "ButtonAddFiles";
			this.ButtonAddFiles.Size = new System.Drawing.Size(75, 23);
			this.ButtonAddFiles.TabIndex = 3;
			this.ButtonAddFiles.Text = "Add &Files...";
			this.ButtonAddFiles.UseVisualStyleBackColor = true;
			this.ButtonAddFiles.Click += new System.EventHandler(this.ButtonAddFiles_Click);
			// 
			// ListBoxFiles
			// 
			this.ListBoxFiles.FormattingEnabled = true;
			this.ListBoxFiles.ItemHeight = 23;
			this.ListBoxFiles.Location = new System.Drawing.Point(16, 38);
			this.ListBoxFiles.Name = "ListBoxFiles";
			this.ListBoxFiles.Size = new System.Drawing.Size(520, 73);
			this.ListBoxFiles.TabIndex = 4;
			this.ListBoxFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListBoxFiles_KeyDown);
			// 
			// LabelFilesData
			// 
			this.LabelFilesData.AutoSize = true;
			this.LabelFilesData.Location = new System.Drawing.Point(12, 127);
			this.LabelFilesData.Name = "LabelFilesData";
			this.LabelFilesData.Size = new System.Drawing.Size(254, 23);
			this.LabelFilesData.TabIndex = 5;
			this.LabelFilesData.Text = "{0} files loaded. Total size: {1} {2}.";
			// 
			// StaticLabelSeparatorA
			// 
			this.StaticLabelSeparatorA.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.StaticLabelSeparatorA.Location = new System.Drawing.Point(16, 144);
			this.StaticLabelSeparatorA.Name = "StaticLabelSeparatorA";
			this.StaticLabelSeparatorA.Size = new System.Drawing.Size(520, 1);
			this.StaticLabelSeparatorA.TabIndex = 6;
			// 
			// StaticLabelBitDepth
			// 
			this.StaticLabelBitDepth.AutoSize = true;
			this.StaticLabelBitDepth.Location = new System.Drawing.Point(13, 149);
			this.StaticLabelBitDepth.Name = "StaticLabelBitDepth";
			this.StaticLabelBitDepth.Size = new System.Drawing.Size(86, 23);
			this.StaticLabelBitDepth.TabIndex = 7;
			this.StaticLabelBitDepth.Text = "Bit Depth:";
			// 
			// ComboBoxBitDepths
			// 
			this.ComboBoxBitDepths.FormattingEnabled = true;
			this.ComboBoxBitDepths.Items.AddRange(new object[] {
            "1bpp",
            "2bpp",
            "4bpp",
            "8bpp",
            "16bpp",
            "24bpp",
            "32bpp"});
			this.ComboBoxBitDepths.Location = new System.Drawing.Point(78, 146);
			this.ComboBoxBitDepths.Name = "ComboBoxBitDepths";
			this.ComboBoxBitDepths.Size = new System.Drawing.Size(121, 31);
			this.ComboBoxBitDepths.TabIndex = 8;
			this.ComboBoxBitDepths.SelectedIndexChanged += new System.EventHandler(this.ComboBoxBitDepths_SelectedIndexChanged);
			// 
			// StaticLabelColorMode
			// 
			this.StaticLabelColorMode.AutoSize = true;
			this.StaticLabelColorMode.Location = new System.Drawing.Point(220, 149);
			this.StaticLabelColorMode.Name = "StaticLabelColorMode";
			this.StaticLabelColorMode.Size = new System.Drawing.Size(104, 23);
			this.StaticLabelColorMode.TabIndex = 9;
			this.StaticLabelColorMode.Text = "Color Mode:";
			// 
			// RadioGrayscale
			// 
			this.RadioGrayscale.AutoSize = true;
			this.RadioGrayscale.Location = new System.Drawing.Point(297, 147);
			this.RadioGrayscale.Name = "RadioGrayscale";
			this.RadioGrayscale.Size = new System.Drawing.Size(107, 27);
			this.RadioGrayscale.TabIndex = 10;
			this.RadioGrayscale.TabStop = true;
			this.RadioGrayscale.Text = "&Grayscale";
			this.RadioGrayscale.UseVisualStyleBackColor = true;
			this.RadioGrayscale.CheckedChanged += new System.EventHandler(this.RadioGrayscale_CheckedChanged);
			// 
			// RadioRGB
			// 
			this.RadioRGB.AutoSize = true;
			this.RadioRGB.Location = new System.Drawing.Point(371, 149);
			this.RadioRGB.Name = "RadioRGB";
			this.RadioRGB.Size = new System.Drawing.Size(67, 27);
			this.RadioRGB.TabIndex = 11;
			this.RadioRGB.TabStop = true;
			this.RadioRGB.Text = "&RGB";
			this.RadioRGB.UseVisualStyleBackColor = true;
			this.RadioRGB.CheckedChanged += new System.EventHandler(this.RadioRGB_CheckedChanged);
			// 
			// RadioARGB
			// 
			this.RadioARGB.AutoSize = true;
			this.RadioARGB.Location = new System.Drawing.Point(297, 170);
			this.RadioARGB.Name = "RadioARGB";
			this.RadioARGB.Size = new System.Drawing.Size(78, 27);
			this.RadioARGB.TabIndex = 12;
			this.RadioARGB.TabStop = true;
			this.RadioARGB.Text = "&ARGB";
			this.RadioARGB.UseVisualStyleBackColor = true;
			this.RadioARGB.CheckedChanged += new System.EventHandler(this.RadioARGB_CheckedChanged);
			// 
			// RadioPaletted
			// 
			this.RadioPaletted.AutoSize = true;
			this.RadioPaletted.Location = new System.Drawing.Point(371, 170);
			this.RadioPaletted.Name = "RadioPaletted";
			this.RadioPaletted.Size = new System.Drawing.Size(97, 27);
			this.RadioPaletted.TabIndex = 13;
			this.RadioPaletted.TabStop = true;
			this.RadioPaletted.Text = "&Paletted";
			this.RadioPaletted.UseVisualStyleBackColor = true;
			this.RadioPaletted.Visible = false;
			this.RadioPaletted.CheckedChanged += new System.EventHandler(this.RadioPaletted_CheckedChanged);
			// 
			// ButtonEditPalette
			// 
			this.ButtonEditPalette.Location = new System.Drawing.Point(444, 149);
			this.ButtonEditPalette.Name = "ButtonEditPalette";
			this.ButtonEditPalette.Size = new System.Drawing.Size(92, 23);
			this.ButtonEditPalette.TabIndex = 14;
			this.ButtonEditPalette.Text = "&Edit Palette...";
			this.ButtonEditPalette.UseVisualStyleBackColor = true;
			this.ButtonEditPalette.Visible = false;
			// 
			// StaticLabelImageWidth
			// 
			this.StaticLabelImageWidth.AutoSize = true;
			this.StaticLabelImageWidth.Location = new System.Drawing.Point(12, 194);
			this.StaticLabelImageWidth.Name = "StaticLabelImageWidth";
			this.StaticLabelImageWidth.Size = new System.Drawing.Size(113, 23);
			this.StaticLabelImageWidth.TabIndex = 15;
			this.StaticLabelImageWidth.Text = "Image Width:";
			// 
			// StaticLabelImageHeight
			// 
			this.StaticLabelImageHeight.AutoSize = true;
			this.StaticLabelImageHeight.Location = new System.Drawing.Point(12, 221);
			this.StaticLabelImageHeight.Name = "StaticLabelImageHeight";
			this.StaticLabelImageHeight.Size = new System.Drawing.Size(118, 23);
			this.StaticLabelImageHeight.TabIndex = 16;
			this.StaticLabelImageHeight.Text = "Image Height:";
			// 
			// TextBoxImageWidth
			// 
			this.TextBoxImageWidth.Location = new System.Drawing.Point(94, 191);
			this.TextBoxImageWidth.Name = "TextBoxImageWidth";
			this.TextBoxImageWidth.Size = new System.Drawing.Size(100, 29);
			this.TextBoxImageWidth.TabIndex = 17;
			this.TextBoxImageWidth.Text = "640";
			this.TextBoxImageWidth.TextChanged += new System.EventHandler(this.TextBoxImageWidth_TextChanged);
			// 
			// TextBoxImageHeight
			// 
			this.TextBoxImageHeight.Location = new System.Drawing.Point(94, 218);
			this.TextBoxImageHeight.Name = "TextBoxImageHeight";
			this.TextBoxImageHeight.Size = new System.Drawing.Size(100, 29);
			this.TextBoxImageHeight.TabIndex = 18;
			this.TextBoxImageHeight.Text = "480";
			this.TextBoxImageHeight.TextChanged += new System.EventHandler(this.TextBoxImageHeight_TextChanged);
			// 
			// LabelImageData
			// 
			this.LabelImageData.AutoSize = true;
			this.LabelImageData.Location = new System.Drawing.Point(94, 247);
			this.LabelImageData.Name = "LabelImageData";
			this.LabelImageData.Size = new System.Drawing.Size(325, 23);
			this.LabelImageData.TabIndex = 19;
			this.LabelImageData.Text = "{0} pixels. Size: {1} {2}. Effective height: {3}.";
			// 
			// LabelOutputFolder
			// 
			this.LabelOutputFolder.AutoSize = true;
			this.LabelOutputFolder.Location = new System.Drawing.Point(207, 198);
			this.LabelOutputFolder.Name = "LabelOutputFolder";
			this.LabelOutputFolder.Size = new System.Drawing.Size(121, 23);
			this.LabelOutputFolder.TabIndex = 20;
			this.LabelOutputFolder.Text = "Output Folder:";
			// 
			// TextOutputFolder
			// 
			this.TextOutputFolder.Location = new System.Drawing.Point(297, 194);
			this.TextOutputFolder.Name = "TextOutputFolder";
			this.TextOutputFolder.Size = new System.Drawing.Size(158, 29);
			this.TextOutputFolder.TabIndex = 21;
			// 
			// ButtonSelectOutputFolder
			// 
			this.ButtonSelectOutputFolder.Location = new System.Drawing.Point(461, 193);
			this.ButtonSelectOutputFolder.Name = "ButtonSelectOutputFolder";
			this.ButtonSelectOutputFolder.Size = new System.Drawing.Size(75, 23);
			this.ButtonSelectOutputFolder.TabIndex = 22;
			this.ButtonSelectOutputFolder.Text = "...";
			this.ButtonSelectOutputFolder.UseVisualStyleBackColor = true;
			this.ButtonSelectOutputFolder.Click += new System.EventHandler(this.ButtonSelectOutputFolder_Click);
			// 
			// ButtonClose
			// 
			this.ButtonClose.Location = new System.Drawing.Point(461, 319);
			this.ButtonClose.Name = "ButtonClose";
			this.ButtonClose.Size = new System.Drawing.Size(75, 23);
			this.ButtonClose.TabIndex = 23;
			this.ButtonClose.Text = "&Close";
			this.ButtonClose.UseVisualStyleBackColor = true;
			this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
			// 
			// ButtonStop
			// 
			this.ButtonStop.Enabled = false;
			this.ButtonStop.Location = new System.Drawing.Point(381, 319);
			this.ButtonStop.Name = "ButtonStop";
			this.ButtonStop.Size = new System.Drawing.Size(75, 23);
			this.ButtonStop.TabIndex = 24;
			this.ButtonStop.Text = "&Stop";
			this.ButtonStop.UseVisualStyleBackColor = true;
			this.ButtonStop.Click += new System.EventHandler(this.ButtonStop_Click);
			// 
			// ButtonGenerate
			// 
			this.ButtonGenerate.Location = new System.Drawing.Point(266, 319);
			this.ButtonGenerate.Name = "ButtonGenerate";
			this.ButtonGenerate.Size = new System.Drawing.Size(109, 23);
			this.ButtonGenerate.TabIndex = 25;
			this.ButtonGenerate.Text = "Generate Pictures";
			this.ButtonGenerate.UseVisualStyleBackColor = true;
			this.ButtonGenerate.Click += new System.EventHandler(this.ButtonGenerate_Click);
			// 
			// StaticLabelSeparatorB
			// 
			this.StaticLabelSeparatorB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.StaticLabelSeparatorB.Location = new System.Drawing.Point(14, 267);
			this.StaticLabelSeparatorB.Name = "StaticLabelSeparatorB";
			this.StaticLabelSeparatorB.Size = new System.Drawing.Size(520, 1);
			this.StaticLabelSeparatorB.TabIndex = 26;
			// 
			// LabelStatus
			// 
			this.LabelStatus.AutoSize = true;
			this.LabelStatus.Location = new System.Drawing.Point(16, 272);
			this.LabelStatus.Name = "LabelStatus";
			this.LabelStatus.Size = new System.Drawing.Size(80, 23);
			this.LabelStatus.TabIndex = 27;
			this.LabelStatus.Text = "Waiting...";
			// 
			// Progress
			// 
			this.Progress.Location = new System.Drawing.Point(19, 289);
			this.Progress.Name = "Progress";
			this.Progress.Size = new System.Drawing.Size(515, 24);
			this.Progress.TabIndex = 28;
			// 
			// OFDAddFile
			// 
			this.OFDAddFile.Filter = "All Files|*.*";
			this.OFDAddFile.Multiselect = true;
			this.OFDAddFile.Title = "Add Files";
			// 
			// FBDAddFolder
			// 
			this.FBDAddFolder.Description = "Select a folder below to add its contents to the output images.";
			// 
			// TextBoxFileEntry
			// 
			this.TextBoxFileEntry.Location = new System.Drawing.Point(52, 10);
			this.TextBoxFileEntry.Name = "TextBoxFileEntry";
			this.TextBoxFileEntry.Size = new System.Drawing.Size(313, 29);
			this.TextBoxFileEntry.TabIndex = 1;
			this.TextBoxFileEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxFileEntry_KeyDown);
			// 
			// FBDOutputFolder
			// 
			this.FBDOutputFolder.Description = "Select the folder to output the result images.";
			// 
			// CheckDrawFileNames
			// 
			this.CheckDrawFileNames.AutoSize = true;
			this.CheckDrawFileNames.Location = new System.Drawing.Point(210, 222);
			this.CheckDrawFileNames.Name = "CheckDrawFileNames";
			this.CheckDrawFileNames.Size = new System.Drawing.Size(163, 27);
			this.CheckDrawFileNames.TabIndex = 29;
			this.CheckDrawFileNames.Text = "Draw File Names";
			this.CheckDrawFileNames.UseVisualStyleBackColor = true;
			this.CheckDrawFileNames.CheckedChanged += new System.EventHandler(this.CheckDrawFileNames_CheckedChanged);
			//
			// CheckDrawAsZoomableTiles
			//
            this.CheckDrawAsZoomableTiles.AutoSize = true;
			this.CheckDrawAsZoomableTiles.Location = new System.Drawing.Point(325, 222);
            this.CheckDrawAsZoomableTiles.Name = "CheckDrawAsZoomableTiles";
			this.CheckDrawAsZoomableTiles.Size = new System.Drawing.Size(163, 27);
            this.CheckDrawAsZoomableTiles.TabIndex = 30;
            this.CheckDrawAsZoomableTiles.Text = "Draw as Zoomable Tiles";
            this.CheckDrawAsZoomableTiles.UseVisualStyleBackColor = true;
			this.CheckDrawAsZoomableTiles.CheckedChanged += new System.EventHandler(this.CheckDrawAsZoomableTiles_CheckedChanged);
			// 
			// LargeFileProcessorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(548, 354);
			this.Controls.Add(this.CheckDrawFileNames);
			this.Controls.Add(this.Progress);
			this.Controls.Add(this.LabelStatus);
			this.Controls.Add(this.StaticLabelSeparatorB);
			this.Controls.Add(this.ButtonGenerate);
			this.Controls.Add(this.ButtonStop);
			this.Controls.Add(this.ButtonClose);
			this.Controls.Add(this.ButtonSelectOutputFolder);
			this.Controls.Add(this.TextOutputFolder);
			this.Controls.Add(this.LabelOutputFolder);
			this.Controls.Add(this.LabelImageData);
			this.Controls.Add(this.TextBoxImageHeight);
			this.Controls.Add(this.TextBoxImageWidth);
			this.Controls.Add(this.StaticLabelImageHeight);
			this.Controls.Add(this.StaticLabelImageWidth);
			this.Controls.Add(this.ButtonEditPalette);
			this.Controls.Add(this.RadioPaletted);
			this.Controls.Add(this.RadioARGB);
			this.Controls.Add(this.RadioRGB);
			this.Controls.Add(this.RadioGrayscale);
			this.Controls.Add(this.StaticLabelColorMode);
			this.Controls.Add(this.ComboBoxBitDepths);
			this.Controls.Add(this.StaticLabelBitDepth);
			this.Controls.Add(this.StaticLabelSeparatorA);
			this.Controls.Add(this.LabelFilesData);
			this.Controls.Add(this.ListBoxFiles);
			this.Controls.Add(this.ButtonAddFiles);
			this.Controls.Add(this.ButtonAddFolder);
			this.Controls.Add(this.TextBoxFileEntry);
			this.Controls.Add(this.StaticLabelFiles);
            this.Controls.Add(this.CheckDrawAsZoomableTiles);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "LargeFileProcessorForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Large File Processor";
			this.Load += new System.EventHandler(this.LargeFileProcessorForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StaticLabelFiles;
        private System.Windows.Forms.Button ButtonAddFolder;
        private System.Windows.Forms.Button ButtonAddFiles;
        private System.Windows.Forms.ListBox ListBoxFiles;
        private System.Windows.Forms.Label LabelFilesData;
        private System.Windows.Forms.Label StaticLabelSeparatorA;
        private System.Windows.Forms.Label StaticLabelBitDepth;
        private System.Windows.Forms.ComboBox ComboBoxBitDepths;
        private System.Windows.Forms.Label StaticLabelColorMode;
        private System.Windows.Forms.RadioButton RadioGrayscale;
        private System.Windows.Forms.RadioButton RadioRGB;
        private System.Windows.Forms.RadioButton RadioARGB;
        private System.Windows.Forms.RadioButton RadioPaletted;
        private System.Windows.Forms.Button ButtonEditPalette;
        private System.Windows.Forms.Label StaticLabelImageWidth;
        private System.Windows.Forms.Label StaticLabelImageHeight;
        private System.Windows.Forms.TextBox TextBoxImageWidth;
        private System.Windows.Forms.TextBox TextBoxImageHeight;
        private System.Windows.Forms.Label LabelImageData;
        private System.Windows.Forms.Label LabelOutputFolder;
        private System.Windows.Forms.TextBox TextOutputFolder;
        private System.Windows.Forms.Button ButtonSelectOutputFolder;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Button ButtonStop;
        private System.Windows.Forms.Button ButtonGenerate;
        private System.Windows.Forms.Label StaticLabelSeparatorB;
        private System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.OpenFileDialog OFDAddFile;
        private System.Windows.Forms.FolderBrowserDialog FBDAddFolder;
        private System.Windows.Forms.TextBox TextBoxFileEntry;
        private System.Windows.Forms.FolderBrowserDialog FBDOutputFolder;
		private System.Windows.Forms.CheckBox CheckDrawFileNames;
        private System.Windows.Forms.CheckBox CheckDrawAsZoomableTiles;
    }
}