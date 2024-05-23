namespace Celarix.Imaging.ImageViewer
{
	partial class MainForm
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
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			TSMain = new ToolStrip();
			TSBOpenFile = new ToolStripButton();
			TSBImageProperties = new ToolStripButton();
			TSS1 = new ToolStripSeparator();
			TSBPreviousFile = new ToolStripButton();
			TSTFileIndex = new ToolStripTextBox();
			TSBNextFile = new ToolStripButton();
			TSS2 = new ToolStripSeparator();
			TSBSortFileName = new ToolStripButton();
			TSSSortByDate = new ToolStripButton();
			TSBSortBySize = new ToolStripButton();
			TSBSortAscending = new ToolStripButton();
			TSBSortDescending = new ToolStripButton();
			TSS3 = new ToolStripSeparator();
			TSBZoomOut = new ToolStripButton();
			TSTZoomLevel = new ToolStripTextBox();
			TSBZoomIn = new ToolStripButton();
			CMSImage = new ContextMenuStrip(components);
			TSMIStandardMode = new ToolStripMenuItem();
			TSMIComicMode = new ToolStripMenuItem();
			TSMIBinaryDrawingMode = new ToolStripMenuItem();
			TSMIBitplaneMode = new ToolStripMenuItem();
			CMSTSS1 = new ToolStripSeparator();
			TSMISortByRGB = new ToolStripMenuItem();
			TSMISortByHSV = new ToolStripMenuItem();
			TSMIUniqueColors = new ToolStripMenuItem();
			TSMIOpenAnimatedControls = new ToolStripMenuItem();
			TSMIBuildRGBGamut = new ToolStripMenuItem();
			TSMIBuildHSVGamut = new ToolStripMenuItem();
			CMSTSS2 = new ToolStripSeparator();
			TSMIZoomModeSelector = new ToolStripMenuItem();
			TSBRotateCCW = new ToolStripButton();
			TSBRotateCW = new ToolStripButton();
			TSS4 = new ToolStripSeparator();
			TSBMarkImage = new ToolStripButton();
			TSBViewMarkedImages = new ToolStripButton();
			SSMain = new StatusStrip();
			TSSImageInfo = new ToolStripStatusLabel();
			TSMain.SuspendLayout();
			CMSImage.SuspendLayout();
			SSMain.SuspendLayout();
			SuspendLayout();
			// 
			// TSMain
			// 
			TSMain.Items.AddRange(new ToolStripItem[] { TSBOpenFile, TSBImageProperties, TSS1, TSBPreviousFile, TSTFileIndex, TSBNextFile, TSS2, TSBSortFileName, TSSSortByDate, TSBSortBySize, TSBSortAscending, TSBSortDescending, TSS3, TSBZoomOut, TSTZoomLevel, TSBZoomIn, TSBRotateCCW, TSBRotateCW, TSS4, TSBMarkImage, TSBViewMarkedImages });
			TSMain.Location = new Point(0, 0);
			TSMain.Name = "TSMain";
			TSMain.Size = new Size(1048, 25);
			TSMain.TabIndex = 0;
			TSMain.Text = "toolStrip1";
			// 
			// TSBOpenFile
			// 
			TSBOpenFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBOpenFile.Image = (Image)resources.GetObject("TSBOpenFile.Image");
			TSBOpenFile.ImageTransparentColor = Color.Magenta;
			TSBOpenFile.Name = "TSBOpenFile";
			TSBOpenFile.Size = new Size(40, 22);
			TSBOpenFile.Text = "Open";
			// 
			// TSBImageProperties
			// 
			TSBImageProperties.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBImageProperties.Image = (Image)resources.GetObject("TSBImageProperties.Image");
			TSBImageProperties.ImageTransparentColor = Color.Magenta;
			TSBImageProperties.Name = "TSBImageProperties";
			TSBImageProperties.Size = new Size(64, 22);
			TSBImageProperties.Text = "Properties";
			// 
			// TSS1
			// 
			TSS1.Name = "TSS1";
			TSS1.Size = new Size(6, 25);
			// 
			// TSBPreviousFile
			// 
			TSBPreviousFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBPreviousFile.Image = (Image)resources.GetObject("TSBPreviousFile.Image");
			TSBPreviousFile.ImageTransparentColor = Color.Magenta;
			TSBPreviousFile.Name = "TSBPreviousFile";
			TSBPreviousFile.Size = new Size(56, 22);
			TSBPreviousFile.Text = "Previous";
			// 
			// TSTFileIndex
			// 
			TSTFileIndex.Name = "TSTFileIndex";
			TSTFileIndex.Size = new Size(80, 25);
			TSTFileIndex.Text = "0 / 0";
			TSTFileIndex.TextBoxTextAlign = HorizontalAlignment.Center;
			// 
			// TSBNextFile
			// 
			TSBNextFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBNextFile.Image = (Image)resources.GetObject("TSBNextFile.Image");
			TSBNextFile.ImageTransparentColor = Color.Magenta;
			TSBNextFile.Name = "TSBNextFile";
			TSBNextFile.Size = new Size(36, 22);
			TSBNextFile.Text = "Next";
			// 
			// TSS2
			// 
			TSS2.Name = "TSS2";
			TSS2.Size = new Size(6, 25);
			// 
			// TSBSortFileName
			// 
			TSBSortFileName.Checked = true;
			TSBSortFileName.CheckState = CheckState.Checked;
			TSBSortFileName.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBSortFileName.Image = (Image)resources.GetObject("TSBSortFileName.Image");
			TSBSortFileName.ImageTransparentColor = Color.Magenta;
			TSBSortFileName.Name = "TSBSortFileName";
			TSBSortFileName.Size = new Size(59, 22);
			TSBSortFileName.Text = "By Name";
			// 
			// TSSSortByDate
			// 
			TSSSortByDate.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSSSortByDate.Image = (Image)resources.GetObject("TSSSortByDate.Image");
			TSSSortByDate.ImageTransparentColor = Color.Magenta;
			TSSSortByDate.Name = "TSSSortByDate";
			TSSSortByDate.Size = new Size(51, 22);
			TSSSortByDate.Text = "By Date";
			// 
			// TSBSortBySize
			// 
			TSBSortBySize.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBSortBySize.Image = (Image)resources.GetObject("TSBSortBySize.Image");
			TSBSortBySize.ImageTransparentColor = Color.Magenta;
			TSBSortBySize.Name = "TSBSortBySize";
			TSBSortBySize.Size = new Size(47, 22);
			TSBSortBySize.Text = "By Size";
			// 
			// TSBSortAscending
			// 
			TSBSortAscending.Checked = true;
			TSBSortAscending.CheckState = CheckState.Checked;
			TSBSortAscending.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBSortAscending.Image = (Image)resources.GetObject("TSBSortAscending.Image");
			TSBSortAscending.ImageTransparentColor = Color.Magenta;
			TSBSortAscending.Name = "TSBSortAscending";
			TSBSortAscending.Size = new Size(33, 22);
			TSBSortAscending.Text = "Asc.";
			// 
			// TSBSortDescending
			// 
			TSBSortDescending.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBSortDescending.Image = (Image)resources.GetObject("TSBSortDescending.Image");
			TSBSortDescending.ImageTransparentColor = Color.Magenta;
			TSBSortDescending.Name = "TSBSortDescending";
			TSBSortDescending.Size = new Size(39, 22);
			TSBSortDescending.Text = "Desc.";
			// 
			// TSS3
			// 
			TSS3.Name = "TSS3";
			TSS3.Size = new Size(6, 25);
			// 
			// TSBZoomOut
			// 
			TSBZoomOut.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBZoomOut.Image = (Image)resources.GetObject("TSBZoomOut.Image");
			TSBZoomOut.ImageTransparentColor = Color.Magenta;
			TSBZoomOut.Name = "TSBZoomOut";
			TSBZoomOut.Size = new Size(66, 22);
			TSBZoomOut.Text = "Zoom Out";
			// 
			// TSTZoomLevel
			// 
			TSTZoomLevel.Name = "TSTZoomLevel";
			TSTZoomLevel.Size = new Size(50, 25);
			TSTZoomLevel.Text = "100%";
			TSTZoomLevel.TextBoxTextAlign = HorizontalAlignment.Center;
			// 
			// TSBZoomIn
			// 
			TSBZoomIn.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBZoomIn.Image = (Image)resources.GetObject("TSBZoomIn.Image");
			TSBZoomIn.ImageTransparentColor = Color.Magenta;
			TSBZoomIn.Name = "TSBZoomIn";
			TSBZoomIn.Size = new Size(56, 22);
			TSBZoomIn.Text = "Zoom In";
			// 
			// CMSImage
			// 
			CMSImage.Items.AddRange(new ToolStripItem[] { TSMIStandardMode, TSMIComicMode, TSMIBinaryDrawingMode, TSMIBitplaneMode, TSMIOpenAnimatedControls, CMSTSS1, TSMISortByRGB, TSMISortByHSV, TSMIBuildRGBGamut, TSMIBuildHSVGamut, TSMIUniqueColors, CMSTSS2, TSMIZoomModeSelector });
			CMSImage.Name = "CMSImage";
			CMSImage.Size = new Size(218, 258);
			// 
			// TSMIStandardMode
			// 
			TSMIStandardMode.Checked = true;
			TSMIStandardMode.CheckState = CheckState.Checked;
			TSMIStandardMode.Name = "TSMIStandardMode";
			TSMIStandardMode.Size = new Size(217, 22);
			TSMIStandardMode.Text = "Standard Mode";
			// 
			// TSMIComicMode
			// 
			TSMIComicMode.Name = "TSMIComicMode";
			TSMIComicMode.Size = new Size(217, 22);
			TSMIComicMode.Text = "Comic Mode";
			// 
			// TSMIBinaryDrawingMode
			// 
			TSMIBinaryDrawingMode.Name = "TSMIBinaryDrawingMode";
			TSMIBinaryDrawingMode.Size = new Size(217, 22);
			TSMIBinaryDrawingMode.Text = "Binary Drawing Mode";
			// 
			// TSMIBitplaneMode
			// 
			TSMIBitplaneMode.Name = "TSMIBitplaneMode";
			TSMIBitplaneMode.Size = new Size(217, 22);
			TSMIBitplaneMode.Text = "Bitplane Mode";
			// 
			// CMSTSS1
			// 
			CMSTSS1.Name = "CMSTSS1";
			CMSTSS1.Size = new Size(214, 6);
			// 
			// TSMISortByRGB
			// 
			TSMISortByRGB.Name = "TSMISortByRGB";
			TSMISortByRGB.Size = new Size(217, 22);
			TSMISortByRGB.Text = "Sort by color (RGB)";
			// 
			// TSMISortByHSV
			// 
			TSMISortByHSV.Name = "TSMISortByHSV";
			TSMISortByHSV.Size = new Size(217, 22);
			TSMISortByHSV.Text = "Sort by color (HSV)";
			// 
			// TSMIUniqueColors
			// 
			TSMIUniqueColors.Name = "TSMIUniqueColors";
			TSMIUniqueColors.Size = new Size(217, 22);
			TSMIUniqueColors.Text = "Unique colors";
			// 
			// TSMIOpenAnimatedControls
			// 
			TSMIOpenAnimatedControls.Enabled = false;
			TSMIOpenAnimatedControls.Name = "TSMIOpenAnimatedControls";
			TSMIOpenAnimatedControls.Size = new Size(217, 22);
			TSMIOpenAnimatedControls.Text = "Animated image controls...";
			// 
			// TSMIBuildRGBGamut
			// 
			TSMIBuildRGBGamut.Name = "TSMIBuildRGBGamut";
			TSMIBuildRGBGamut.Size = new Size(217, 22);
			TSMIBuildRGBGamut.Text = "Build color gamut (RGB)";
			// 
			// TSMIBuildHSVGamut
			// 
			TSMIBuildHSVGamut.Name = "TSMIBuildHSVGamut";
			TSMIBuildHSVGamut.Size = new Size(217, 22);
			TSMIBuildHSVGamut.Text = "Build color gamut (HSV)";
			// 
			// CMSTSS2
			// 
			CMSTSS2.Name = "CMSTSS2";
			CMSTSS2.Size = new Size(214, 6);
			// 
			// TSMIZoomModeSelector
			// 
			TSMIZoomModeSelector.Name = "TSMIZoomModeSelector";
			TSMIZoomModeSelector.Size = new Size(217, 22);
			TSMIZoomModeSelector.Text = "Non-smoothing zoom";
			// 
			// TSBRotateCCW
			// 
			TSBRotateCCW.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBRotateCCW.Image = (Image)resources.GetObject("TSBRotateCCW.Image");
			TSBRotateCCW.ImageTransparentColor = Color.Magenta;
			TSBRotateCCW.Name = "TSBRotateCCW";
			TSBRotateCCW.Size = new Size(38, 22);
			TSBRotateCCW.Text = "CCW";
			// 
			// TSBRotateCW
			// 
			TSBRotateCW.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBRotateCW.Image = (Image)resources.GetObject("TSBRotateCW.Image");
			TSBRotateCW.ImageTransparentColor = Color.Magenta;
			TSBRotateCW.Name = "TSBRotateCW";
			TSBRotateCW.Size = new Size(30, 22);
			TSBRotateCW.Text = "CW";
			// 
			// TSS4
			// 
			TSS4.Name = "TSS4";
			TSS4.Size = new Size(6, 25);
			// 
			// TSBMarkImage
			// 
			TSBMarkImage.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBMarkImage.Image = (Image)resources.GetObject("TSBMarkImage.Image");
			TSBMarkImage.ImageTransparentColor = Color.Magenta;
			TSBMarkImage.Name = "TSBMarkImage";
			TSBMarkImage.Size = new Size(38, 22);
			TSBMarkImage.Text = "Mark";
			// 
			// TSBViewMarkedImages
			// 
			TSBViewMarkedImages.DisplayStyle = ToolStripItemDisplayStyle.Text;
			TSBViewMarkedImages.Image = (Image)resources.GetObject("TSBViewMarkedImages.Image");
			TSBViewMarkedImages.ImageTransparentColor = Color.Magenta;
			TSBViewMarkedImages.Name = "TSBViewMarkedImages";
			TSBViewMarkedImages.Size = new Size(88, 22);
			TSBViewMarkedImages.Text = "View Marked...";
			// 
			// SSMain
			// 
			SSMain.Items.AddRange(new ToolStripItem[] { TSSImageInfo });
			SSMain.Location = new Point(0, 428);
			SSMain.Name = "SSMain";
			SSMain.Size = new Size(1048, 22);
			SSMain.TabIndex = 1;
			SSMain.Text = "statusStrip1";
			// 
			// TSSImageInfo
			// 
			TSSImageInfo.Name = "TSSImageInfo";
			TSSImageInfo.Size = new Size(595, 17);
			TSSImageInfo.Text = "Browsing files | 0 of 0 | 0 x 0 @ 0 BPP | 0 pixels | 0.00 KB (0.00 KB uncompressed, N/A%) | 1970-01-01 12:00:00 AM";
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1048, 450);
			Controls.Add(SSMain);
			Controls.Add(TSMain);
			Name = "MainForm";
			Text = "ImageViewer";
			TSMain.ResumeLayout(false);
			TSMain.PerformLayout();
			CMSImage.ResumeLayout(false);
			SSMain.ResumeLayout(false);
			SSMain.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ToolStrip TSMain;
		private ToolStripButton TSBOpenFile;
		private ToolStripButton TSBImageProperties;
		private ToolStripSeparator TSS1;
		private ToolStripButton TSBPreviousFile;
		private ToolStripTextBox TSTFileIndex;
		private ToolStripButton TSBNextFile;
		private ToolStripSeparator TSS2;
		private ToolStripButton TSBSortFileName;
		private ToolStripButton TSSSortByDate;
		private ToolStripButton TSBSortBySize;
		private ToolStripButton TSBSortAscending;
		private ToolStripButton TSBSortDescending;
		private ToolStripSeparator TSS3;
		private ToolStripButton TSBZoomOut;
		private ToolStripTextBox TSTZoomLevel;
		private ToolStripButton TSBZoomIn;
		private ContextMenuStrip CMSImage;
		private ToolStripMenuItem TSMIStandardMode;
		private ToolStripMenuItem TSMIComicMode;
		private ToolStripMenuItem TSMIBinaryDrawingMode;
		private ToolStripMenuItem TSMIBitplaneMode;
		private ToolStripSeparator CMSTSS1;
		private ToolStripMenuItem TSMISortByRGB;
		private ToolStripMenuItem TSMISortByHSV;
		private ToolStripMenuItem TSMIUniqueColors;
		private ToolStripMenuItem TSMIOpenAnimatedControls;
		private ToolStripMenuItem TSMIBuildRGBGamut;
		private ToolStripMenuItem TSMIBuildHSVGamut;
		private ToolStripSeparator CMSTSS2;
		private ToolStripMenuItem TSMIZoomModeSelector;
		private ToolStripButton TSBRotateCCW;
		private ToolStripButton TSBRotateCW;
		private ToolStripSeparator TSS4;
		private ToolStripButton TSBMarkImage;
		private ToolStripButton TSBViewMarkedImages;
		private StatusStrip SSMain;
		private ToolStripStatusLabel TSSImageInfo;
	}
}
