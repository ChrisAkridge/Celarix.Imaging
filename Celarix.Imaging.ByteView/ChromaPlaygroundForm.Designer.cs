namespace Celarix.Imaging.ByteView
{
	partial class ChromaPlaygroundForm
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
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ChromaPlaygroundForm));
			TSMain = new System.Windows.Forms.ToolStrip();
			TSBOpenImage = new System.Windows.Forms.ToolStripButton();
			TSBSaveAs = new System.Windows.Forms.ToolStripButton();
			TSBLoadFromByteView = new System.Windows.Forms.ToolStripButton();
			TSSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			TSBKeepResult = new System.Windows.Forms.ToolStripButton();
			TSBResetEffect = new System.Windows.Forms.ToolStripButton();
			TSBShowOriginal = new System.Windows.Forms.ToolStripButton();
			TSSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			TSBFitToWindow = new System.Windows.Forms.ToolStripButton();
			TSDDExportSignal = new System.Windows.Forms.ToolStripDropDownButton();
			TSBProcessVideo = new System.Windows.Forms.ToolStripButton();
			TSMISaveNTSCAudio = new System.Windows.Forms.ToolStripMenuItem();
			TSMISaveSimplerAudio = new System.Windows.Forms.ToolStripMenuItem();
			StatusMain = new System.Windows.Forms.StatusStrip();
			TSSMain = new System.Windows.Forms.ToolStripStatusLabel();
			SplitMain = new System.Windows.Forms.SplitContainer();
			GroupEffect = new System.Windows.Forms.GroupBox();
			TableOptions = new System.Windows.Forms.TableLayoutPanel();
			LabelEffect = new System.Windows.Forms.Label();
			ComboEffect = new System.Windows.Forms.ComboBox();
			LabelEffectDescription = new System.Windows.Forms.Label();
			LabelParameter1 = new System.Windows.Forms.Label();
			ComboParameter1 = new System.Windows.Forms.ComboBox();
			LabelParameter2 = new System.Windows.Forms.Label();
			ComboParameter2 = new System.Windows.Forms.ComboBox();
			LabelBitIndex = new System.Windows.Forms.Label();
			TrackBitIndex = new System.Windows.Forms.TrackBar();
			LabelSignalStats = new System.Windows.Forms.Label();
			CheckDither = new System.Windows.Forms.CheckBox();
			PanelImage = new System.Windows.Forms.Panel();
			PictureMain = new PixelatedPictureBox();
			OFDOpenImage = new System.Windows.Forms.OpenFileDialog();
			SFDSaveImage = new System.Windows.Forms.SaveFileDialog();
			SFDSaveAnalogSignal = new System.Windows.Forms.SaveFileDialog();
			TSMain.SuspendLayout();
			StatusMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)SplitMain).BeginInit();
			SplitMain.Panel1.SuspendLayout();
			SplitMain.Panel2.SuspendLayout();
			SplitMain.SuspendLayout();
			GroupEffect.SuspendLayout();
			TableOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)TrackBitIndex).BeginInit();
			PanelImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PictureMain).BeginInit();
			SuspendLayout();
			//
			// TSMain
			//
			TSMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TSBOpenImage, TSBSaveAs, TSBLoadFromByteView, TSSeparator1, TSBKeepResult, TSBResetEffect, TSBShowOriginal, TSSeparator2, TSBFitToWindow, TSDDExportSignal, TSBProcessVideo });
			TSMain.Location = new System.Drawing.Point(0, 0);
			TSMain.Name = "TSMain";
			TSMain.Size = new System.Drawing.Size(1000, 25);
			TSMain.TabIndex = 0;
			//
			// TSBOpenImage
			//
			TSBOpenImage.Image = (System.Drawing.Image)resources.GetObject("TSBOpenImage.Image");
			TSBOpenImage.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBOpenImage.Name = "TSBOpenImage";
			TSBOpenImage.Text = "Open Image...";
			TSBOpenImage.Click += TSBOpenImage_Click;
			//
			// TSBSaveAs
			//
			TSBSaveAs.Image = (System.Drawing.Image)resources.GetObject("TSBSaveAs.Image");
			TSBSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBSaveAs.Name = "TSBSaveAs";
			TSBSaveAs.Text = "Save As...";
			TSBSaveAs.ToolTipText = "Save the image currently shown";
			TSBSaveAs.Click += TSBSaveAs_Click;
			//
			// TSBLoadFromByteView
			//
			TSBLoadFromByteView.Image = (System.Drawing.Image)resources.GetObject("TSBLoadFromByteView.Image");
			TSBLoadFromByteView.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBLoadFromByteView.Name = "TSBLoadFromByteView";
			TSBLoadFromByteView.Text = "Load from ByteView";
			TSBLoadFromByteView.Click += TSBLoadFromByteView_Click;
			//
			// TSSeparator1
			//
			TSSeparator1.Name = "TSSeparator1";
			//
			// TSBKeepResult
			//
			TSBKeepResult.Image = (System.Drawing.Image)resources.GetObject("TSBUseCurrent.Image");
			TSBKeepResult.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBKeepResult.Name = "TSBKeepResult";
			TSBKeepResult.Text = "Keep Result";
			TSBKeepResult.ToolTipText = "Make the previewed result the new source image, so another effect can be stacked on it";
			TSBKeepResult.Click += TSBKeepResult_Click;
			//
			// TSBResetEffect
			//
			TSBResetEffect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBResetEffect.Name = "TSBResetEffect";
			TSBResetEffect.Text = "Revert to Loaded Image";
			TSBResetEffect.ToolTipText = "Discard every kept result and go back to the image as it was loaded";
			TSBResetEffect.Click += TSBRevert_Click;
			//
			// TSBShowOriginal
			//
			TSBShowOriginal.CheckOnClick = true;
			TSBShowOriginal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBShowOriginal.Name = "TSBShowOriginal";
			TSBShowOriginal.Text = "Show Source";
			TSBShowOriginal.ToolTipText = "Compare: show the source image instead of the effect's result";
			TSBShowOriginal.CheckedChanged += TSBShowOriginal_CheckedChanged;
			//
			// TSSeparator2
			//
			TSSeparator2.Name = "TSSeparator2";
			//
			// TSBFitToWindow
			//
			TSBFitToWindow.Checked = true;
			TSBFitToWindow.CheckOnClick = true;
			TSBFitToWindow.CheckState = System.Windows.Forms.CheckState.Checked;
			TSBFitToWindow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBFitToWindow.Name = "TSBFitToWindow";
			TSBFitToWindow.Text = "Fit to Window";
			TSBFitToWindow.ToolTipText = "Off shows the image at actual size, with scroll bars";
			TSBFitToWindow.CheckedChanged += TSBFitToWindow_CheckedChanged;
			//
			// TSDDExportSignal
			//
			TSDDExportSignal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { TSMISaveNTSCAudio, TSMISaveSimplerAudio });
			TSDDExportSignal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSDDExportSignal.Name = "TSDDExportSignal";
			TSDDExportSignal.Text = "Export Analog Signal";
			//
			// TSBProcessVideo
			// 
			TSBProcessVideo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBProcessVideo.Name = "TSBProcessVideo";
			TSBProcessVideo.Text = "Process Video...";
			TSBProcessVideo.ToolTipText = "Apply the current effect to every frame of a video file";
			TSBProcessVideo.Click += TSBProcessVideo_Click;
			// 
			// TSMISaveNTSCAudio
			//
			TSMISaveNTSCAudio.Name = "TSMISaveNTSCAudio";
			TSMISaveNTSCAudio.Text = "Mock-NTSC audio...";
			TSMISaveNTSCAudio.Click += TSMISaveNTSCAudio_Click;
			//
			// TSMISaveSimplerAudio
			//
			TSMISaveSimplerAudio.Name = "TSMISaveSimplerAudio";
			TSMISaveSimplerAudio.Text = "Simpler audio...";
			TSMISaveSimplerAudio.Click += TSMISaveSimplerAudio_Click;
			//
			// StatusMain
			//
			StatusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TSSMain });
			StatusMain.Location = new System.Drawing.Point(0, 678);
			StatusMain.Name = "StatusMain";
			StatusMain.Size = new System.Drawing.Size(1000, 22);
			StatusMain.TabIndex = 2;
			//
			// TSSMain
			//
			TSSMain.Name = "TSSMain";
			TSSMain.Text = "Open an image to begin.";
			//
			// SplitMain
			//
			SplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			SplitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			SplitMain.Location = new System.Drawing.Point(0, 25);
			SplitMain.Name = "SplitMain";
			SplitMain.Panel1.Controls.Add(GroupEffect);
			SplitMain.Panel1.Padding = new System.Windows.Forms.Padding(8);
			SplitMain.Panel2.Controls.Add(PanelImage);
			SplitMain.Size = new System.Drawing.Size(1000, 653);
			SplitMain.SplitterDistance = 320;
			SplitMain.TabIndex = 1;
			//
			// GroupEffect
			//
			GroupEffect.Controls.Add(TableOptions);
			GroupEffect.Dock = System.Windows.Forms.DockStyle.Fill;
			GroupEffect.Name = "GroupEffect";
			GroupEffect.Padding = new System.Windows.Forms.Padding(8);
			GroupEffect.TabIndex = 0;
			GroupEffect.TabStop = false;
			GroupEffect.Text = "Effect";
			//
			// TableOptions
			//
			TableOptions.AutoScroll = true;
			TableOptions.ColumnCount = 1;
			TableOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			TableOptions.Controls.Add(LabelEffect, 0, 0);
			TableOptions.Controls.Add(ComboEffect, 0, 1);
			TableOptions.Controls.Add(LabelEffectDescription, 0, 2);
			TableOptions.Controls.Add(LabelParameter1, 0, 3);
			TableOptions.Controls.Add(ComboParameter1, 0, 4);
			TableOptions.Controls.Add(LabelParameter2, 0, 5);
			TableOptions.Controls.Add(ComboParameter2, 0, 6);
			TableOptions.Controls.Add(LabelBitIndex, 0, 7);
			TableOptions.Controls.Add(TrackBitIndex, 0, 8);
			TableOptions.Controls.Add(CheckDither, 0, 9);
			TableOptions.Controls.Add(LabelSignalStats, 0, 10);
			TableOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			TableOptions.Name = "TableOptions";
			TableOptions.RowCount = 12;
			for (int i = 0; i < 11; i++)
			{
				TableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle());
			}
			TableOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			TableOptions.TabIndex = 0;
			//
			// LabelEffect
			//
			LabelEffect.AutoSize = true;
			LabelEffect.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			LabelEffect.Name = "LabelEffect";
			LabelEffect.Text = "Effect:";
			//
			// ComboEffect
			//
			ComboEffect.Dock = System.Windows.Forms.DockStyle.Fill;
			ComboEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ComboEffect.Name = "ComboEffect";
			ComboEffect.TabIndex = 0;
			ComboEffect.SelectedIndexChanged += ComboEffect_SelectedIndexChanged;
			//
			// LabelEffectDescription
			//
			LabelEffectDescription.AutoSize = true;
			LabelEffectDescription.ForeColor = System.Drawing.SystemColors.GrayText;
			LabelEffectDescription.Margin = new System.Windows.Forms.Padding(3, 6, 3, 12);
			LabelEffectDescription.MaximumSize = new System.Drawing.Size(270, 0);
			LabelEffectDescription.Name = "LabelEffectDescription";
			//
			// LabelParameter1
			//
			LabelParameter1.AutoSize = true;
			LabelParameter1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			LabelParameter1.Name = "LabelParameter1";
			//
			// ComboParameter1
			//
			ComboParameter1.Dock = System.Windows.Forms.DockStyle.Fill;
			ComboParameter1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ComboParameter1.Name = "ComboParameter1";
			ComboParameter1.TabIndex = 1;
			ComboParameter1.SelectedIndexChanged += ComboParameter1_SelectedIndexChanged;
			//
			// LabelParameter2
			//
			LabelParameter2.AutoSize = true;
			LabelParameter2.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
			LabelParameter2.Name = "LabelParameter2";
			//
			// ComboParameter2
			//
			ComboParameter2.Dock = System.Windows.Forms.DockStyle.Fill;
			ComboParameter2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ComboParameter2.Name = "ComboParameter2";
			ComboParameter2.TabIndex = 2;
			ComboParameter2.SelectedIndexChanged += ComboParameter2_SelectedIndexChanged;
			//
			// LabelBitIndex
			//
			LabelBitIndex.AutoSize = true;
			LabelBitIndex.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
			LabelBitIndex.Name = "LabelBitIndex";
			//
			// TrackBitIndex
			//
			TrackBitIndex.Dock = System.Windows.Forms.DockStyle.Fill;
			TrackBitIndex.LargeChange = 1;
			TrackBitIndex.Maximum = 7;
			TrackBitIndex.Name = "TrackBitIndex";
			TrackBitIndex.TabIndex = 3;
			TrackBitIndex.Value = 7;
			TrackBitIndex.ValueChanged += TrackBitIndex_ValueChanged;
			//
			// CheckDither
			// 
			CheckDither.AutoSize = true;
			CheckDither.Margin = new System.Windows.Forms.Padding(3, 12, 3, 0);
			CheckDither.Name = "CheckDither";
			CheckDither.TabIndex = 4;
			CheckDither.Text = "Dither (Floyd-Steinberg)";
			CheckDither.UseVisualStyleBackColor = true;
			CheckDither.CheckedChanged += CheckDither_CheckedChanged;
			// 
			// LabelSignalStats
			//
			LabelSignalStats.AutoSize = true;
			LabelSignalStats.ForeColor = System.Drawing.SystemColors.GrayText;
			LabelSignalStats.Margin = new System.Windows.Forms.Padding(3, 18, 3, 0);
			LabelSignalStats.Name = "LabelSignalStats";
			//
			// PanelImage
			//
			PanelImage.AutoScroll = true;
			PanelImage.BackColor = System.Drawing.SystemColors.ControlDark;
			PanelImage.Controls.Add(PictureMain);
			PanelImage.Dock = System.Windows.Forms.DockStyle.Fill;
			PanelImage.Name = "PanelImage";
			PanelImage.TabIndex = 0;
			//
			// PictureMain
			//
			PictureMain.Dock = System.Windows.Forms.DockStyle.Fill;
			PictureMain.Name = "PictureMain";
			PictureMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			PictureMain.TabStop = false;
			//
			// OFDOpenImage
			//
			OFDOpenImage.Filter = "All files|*.*";
			OFDOpenImage.Title = "ByteView - Open Image";
			//
			// SFDSaveImage
			//
			SFDSaveImage.DefaultExt = "png";
			SFDSaveImage.Filter = "Portable Network Graphics|*.png|All files|*.*";
			SFDSaveImage.Title = "ByteView - Save Image";
			//
			// SFDSaveAnalogSignal
			//
			SFDSaveAnalogSignal.DefaultExt = "raw";
			SFDSaveAnalogSignal.Filter = "Raw Audio (mono, 48kHz, single-precision float)|*.raw|All files|*.*";
			SFDSaveAnalogSignal.Title = "ByteView - Save Analog Signal";
			//
			// ChromaPlaygroundForm
			//
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(1000, 700);
			Controls.Add(SplitMain);
			Controls.Add(StatusMain);
			Controls.Add(TSMain);
			MinimumSize = new System.Drawing.Size(640, 480);
			Name = "ChromaPlaygroundForm";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Chroma Playground";
			TSMain.ResumeLayout(false);
			TSMain.PerformLayout();
			StatusMain.ResumeLayout(false);
			StatusMain.PerformLayout();
			SplitMain.Panel1.ResumeLayout(false);
			SplitMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)SplitMain).EndInit();
			SplitMain.ResumeLayout(false);
			GroupEffect.ResumeLayout(false);
			TableOptions.ResumeLayout(false);
			TableOptions.PerformLayout();
			((System.ComponentModel.ISupportInitialize)TrackBitIndex).EndInit();
			PanelImage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PictureMain).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.ToolStrip TSMain;
		private System.Windows.Forms.ToolStripButton TSBOpenImage;
		private System.Windows.Forms.ToolStripButton TSBSaveAs;
		private System.Windows.Forms.ToolStripButton TSBLoadFromByteView;
		private System.Windows.Forms.ToolStripSeparator TSSeparator1;
		private System.Windows.Forms.ToolStripButton TSBKeepResult;
		private System.Windows.Forms.ToolStripButton TSBResetEffect;
		private System.Windows.Forms.ToolStripButton TSBShowOriginal;
		private System.Windows.Forms.ToolStripSeparator TSSeparator2;
		private System.Windows.Forms.ToolStripButton TSBFitToWindow;
		private System.Windows.Forms.ToolStripDropDownButton TSDDExportSignal;
		private System.Windows.Forms.ToolStripButton TSBProcessVideo;
		private System.Windows.Forms.ToolStripMenuItem TSMISaveNTSCAudio;
		private System.Windows.Forms.ToolStripMenuItem TSMISaveSimplerAudio;
		private System.Windows.Forms.StatusStrip StatusMain;
		private System.Windows.Forms.ToolStripStatusLabel TSSMain;
		private System.Windows.Forms.SplitContainer SplitMain;
		private System.Windows.Forms.GroupBox GroupEffect;
		private System.Windows.Forms.TableLayoutPanel TableOptions;
		private System.Windows.Forms.Label LabelEffect;
		private System.Windows.Forms.ComboBox ComboEffect;
		private System.Windows.Forms.Label LabelEffectDescription;
		private System.Windows.Forms.Label LabelParameter1;
		private System.Windows.Forms.ComboBox ComboParameter1;
		private System.Windows.Forms.Label LabelParameter2;
		private System.Windows.Forms.ComboBox ComboParameter2;
		private System.Windows.Forms.Label LabelBitIndex;
		private System.Windows.Forms.TrackBar TrackBitIndex;
		private System.Windows.Forms.Label LabelSignalStats;
		private System.Windows.Forms.CheckBox CheckDither;
		private System.Windows.Forms.Panel PanelImage;
		private PixelatedPictureBox PictureMain;
		private System.Windows.Forms.OpenFileDialog OFDOpenImage;
		private System.Windows.Forms.SaveFileDialog SFDSaveImage;
		private System.Windows.Forms.SaveFileDialog SFDSaveAnalogSignal;
	}
}
