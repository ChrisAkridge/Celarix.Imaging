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
			System.Windows.Forms.TabControl TabsOptions;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ChromaPlaygroundForm));
			TabPageColorChannels = new System.Windows.Forms.TabPage();
			ButtonColorChannelReset = new System.Windows.Forms.Button();
			ButtonSetColorChannel = new System.Windows.Forms.Button();
			RadioChrominance = new System.Windows.Forms.RadioButton();
			RadioCr = new System.Windows.Forms.RadioButton();
			RadioCb = new System.Windows.Forms.RadioButton();
			RadioLuminance = new System.Windows.Forms.RadioButton();
			RadioAlphaChannel = new System.Windows.Forms.RadioButton();
			RadioBlueChannel = new System.Windows.Forms.RadioButton();
			RadioGreenChannel = new System.Windows.Forms.RadioButton();
			RadioRedChannel = new System.Windows.Forms.RadioButton();
			TabPageBitPlanes = new System.Windows.Forms.TabPage();
			StaticLabelBitPlaneExplanation = new System.Windows.Forms.Label();
			NUDBitPlaneIndex = new System.Windows.Forms.NumericUpDown();
			StaticLabelBitPlaneIndex = new System.Windows.Forms.Label();
			RadioBitPlaneAlpha = new System.Windows.Forms.RadioButton();
			RadioBitPlaneBlue = new System.Windows.Forms.RadioButton();
			RadioBitPlaneGreen = new System.Windows.Forms.RadioButton();
			RadioBitPlaneRed = new System.Windows.Forms.RadioButton();
			TabPageChromaSubsampling = new System.Windows.Forms.TabPage();
			StaticLabelChromaSubsamplingExplanation = new System.Windows.Forms.Label();
			ButtonChromaSubsamplingSet = new System.Windows.Forms.Button();
			Radio411 = new System.Windows.Forms.RadioButton();
			Radio420 = new System.Windows.Forms.RadioButton();
			Radio422 = new System.Windows.Forms.RadioButton();
			Radio444 = new System.Windows.Forms.RadioButton();
			TabPageReduceBitDepth = new System.Windows.Forms.TabPage();
			StaticLabelReduceBitDepthExplanation = new System.Windows.Forms.Label();
			ButtonReduceBitDepth = new System.Windows.Forms.Button();
			Radio1BPPGrayscale = new System.Windows.Forms.RadioButton();
			Radio2BPPTop4 = new System.Windows.Forms.RadioButton();
			Radio1BPPTop2 = new System.Windows.Forms.RadioButton();
			Radio2BPPGrayscale = new System.Windows.Forms.RadioButton();
			Radio4BPPGrayscale = new System.Windows.Forms.RadioButton();
			Radio8BPPGrayscale = new System.Windows.Forms.RadioButton();
			Radio4BPPTop16 = new System.Windows.Forms.RadioButton();
			Radio4bppRGB121 = new System.Windows.Forms.RadioButton();
			Radio8BPPTop256 = new System.Windows.Forms.RadioButton();
			Radio8bppRGB332 = new System.Windows.Forms.RadioButton();
			Radio16bppTop65536 = new System.Windows.Forms.RadioButton();
			Radio16BPPRGB565 = new System.Windows.Forms.RadioButton();
			TabPageAnalogSignal = new System.Windows.Forms.TabPage();
			ButtonSaveSimplerAudio = new System.Windows.Forms.Button();
			ButtonSaveNTSCAudio = new System.Windows.Forms.Button();
			LabelAnalogSignalStats = new System.Windows.Forms.Label();
			TSMain = new System.Windows.Forms.ToolStrip();
			TSBOpenImage = new System.Windows.Forms.ToolStripButton();
			TSBSaveAs = new System.Windows.Forms.ToolStripButton();
			TSSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			TSBConvertToRGB = new System.Windows.Forms.ToolStripButton();
			TSBConvertToYCbCr = new System.Windows.Forms.ToolStripButton();
			TSBUseCurrent = new System.Windows.Forms.ToolStripButton();
			StatusMain = new System.Windows.Forms.StatusStrip();
			TSSMain = new System.Windows.Forms.ToolStripStatusLabel();
			panel1 = new System.Windows.Forms.Panel();
			PictureMain = new System.Windows.Forms.PictureBox();
			OFDOpenImage = new System.Windows.Forms.OpenFileDialog();
			SFDSaveImage = new System.Windows.Forms.SaveFileDialog();
			SFDSaveAnalogSignal = new System.Windows.Forms.SaveFileDialog();
			RadioValueChannel = new System.Windows.Forms.RadioButton();
			RadioSaturationChannel = new System.Windows.Forms.RadioButton();
			RadioHueChannel = new System.Windows.Forms.RadioButton();
			TabsOptions = new System.Windows.Forms.TabControl();
			TabsOptions.SuspendLayout();
			TabPageColorChannels.SuspendLayout();
			TabPageBitPlanes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)NUDBitPlaneIndex).BeginInit();
			TabPageChromaSubsampling.SuspendLayout();
			TabPageReduceBitDepth.SuspendLayout();
			TabPageAnalogSignal.SuspendLayout();
			TSMain.SuspendLayout();
			StatusMain.SuspendLayout();
			panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PictureMain).BeginInit();
			SuspendLayout();
			// 
			// TabsOptions
			// 
			TabsOptions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			TabsOptions.Controls.Add(TabPageColorChannels);
			TabsOptions.Controls.Add(TabPageBitPlanes);
			TabsOptions.Controls.Add(TabPageChromaSubsampling);
			TabsOptions.Controls.Add(TabPageReduceBitDepth);
			TabsOptions.Controls.Add(TabPageAnalogSignal);
			TabsOptions.Location = new System.Drawing.Point(12, 28);
			TabsOptions.Name = "TabsOptions";
			TabsOptions.SelectedIndex = 0;
			TabsOptions.Size = new System.Drawing.Size(785, 134);
			TabsOptions.TabIndex = 0;
			// 
			// TabPageColorChannels
			// 
			TabPageColorChannels.Controls.Add(RadioValueChannel);
			TabPageColorChannels.Controls.Add(RadioSaturationChannel);
			TabPageColorChannels.Controls.Add(RadioHueChannel);
			TabPageColorChannels.Controls.Add(ButtonColorChannelReset);
			TabPageColorChannels.Controls.Add(ButtonSetColorChannel);
			TabPageColorChannels.Controls.Add(RadioChrominance);
			TabPageColorChannels.Controls.Add(RadioCr);
			TabPageColorChannels.Controls.Add(RadioCb);
			TabPageColorChannels.Controls.Add(RadioLuminance);
			TabPageColorChannels.Controls.Add(RadioAlphaChannel);
			TabPageColorChannels.Controls.Add(RadioBlueChannel);
			TabPageColorChannels.Controls.Add(RadioGreenChannel);
			TabPageColorChannels.Controls.Add(RadioRedChannel);
			TabPageColorChannels.Location = new System.Drawing.Point(4, 24);
			TabPageColorChannels.Name = "TabPageColorChannels";
			TabPageColorChannels.Padding = new System.Windows.Forms.Padding(3);
			TabPageColorChannels.Size = new System.Drawing.Size(777, 106);
			TabPageColorChannels.TabIndex = 0;
			TabPageColorChannels.Text = "Color Channels";
			TabPageColorChannels.UseVisualStyleBackColor = true;
			// 
			// ButtonColorChannelReset
			// 
			ButtonColorChannelReset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonColorChannelReset.Location = new System.Drawing.Point(696, 48);
			ButtonColorChannelReset.Name = "ButtonColorChannelReset";
			ButtonColorChannelReset.Size = new System.Drawing.Size(75, 23);
			ButtonColorChannelReset.TabIndex = 9;
			ButtonColorChannelReset.Text = "Reset";
			ButtonColorChannelReset.UseVisualStyleBackColor = true;
			ButtonColorChannelReset.Click += ButtonColorChannelReset_Click;
			// 
			// ButtonSetColorChannel
			// 
			ButtonSetColorChannel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonSetColorChannel.Location = new System.Drawing.Point(696, 77);
			ButtonSetColorChannel.Name = "ButtonSetColorChannel";
			ButtonSetColorChannel.Size = new System.Drawing.Size(75, 23);
			ButtonSetColorChannel.TabIndex = 2;
			ButtonSetColorChannel.Text = "Set";
			ButtonSetColorChannel.UseVisualStyleBackColor = true;
			ButtonSetColorChannel.Click += ButtonSetColorChannel_Click;
			// 
			// RadioChrominance
			// 
			RadioChrominance.AutoSize = true;
			RadioChrominance.Location = new System.Drawing.Point(73, 81);
			RadioChrominance.Name = "RadioChrominance";
			RadioChrominance.Size = new System.Drawing.Size(159, 19);
			RadioChrominance.TabIndex = 7;
			RadioChrominance.TabStop = true;
			RadioChrominance.Text = "Cb and Cr (chrominance)";
			RadioChrominance.UseVisualStyleBackColor = true;
			// 
			// RadioCr
			// 
			RadioCr.AutoSize = true;
			RadioCr.Location = new System.Drawing.Point(73, 56);
			RadioCr.Name = "RadioCr";
			RadioCr.Size = new System.Drawing.Size(132, 19);
			RadioCr.TabIndex = 6;
			RadioCr.TabStop = true;
			RadioCr.Text = "Cr (red - luminance)";
			RadioCr.UseVisualStyleBackColor = true;
			// 
			// RadioCb
			// 
			RadioCb.AutoSize = true;
			RadioCb.Location = new System.Drawing.Point(73, 31);
			RadioCb.Name = "RadioCb";
			RadioCb.Size = new System.Drawing.Size(141, 19);
			RadioCb.TabIndex = 5;
			RadioCb.TabStop = true;
			RadioCb.Text = "Cb (blue - luminance)";
			RadioCb.UseVisualStyleBackColor = true;
			// 
			// RadioLuminance
			// 
			RadioLuminance.AutoSize = true;
			RadioLuminance.Location = new System.Drawing.Point(73, 6);
			RadioLuminance.Name = "RadioLuminance";
			RadioLuminance.Size = new System.Drawing.Size(99, 19);
			RadioLuminance.TabIndex = 4;
			RadioLuminance.TabStop = true;
			RadioLuminance.Text = "Y (luminance)";
			RadioLuminance.UseVisualStyleBackColor = true;
			// 
			// RadioAlphaChannel
			// 
			RadioAlphaChannel.AutoSize = true;
			RadioAlphaChannel.Location = new System.Drawing.Point(6, 81);
			RadioAlphaChannel.Name = "RadioAlphaChannel";
			RadioAlphaChannel.Size = new System.Drawing.Size(56, 19);
			RadioAlphaChannel.TabIndex = 3;
			RadioAlphaChannel.TabStop = true;
			RadioAlphaChannel.Text = "Alpha";
			RadioAlphaChannel.UseVisualStyleBackColor = true;
			// 
			// RadioBlueChannel
			// 
			RadioBlueChannel.AutoSize = true;
			RadioBlueChannel.Location = new System.Drawing.Point(6, 56);
			RadioBlueChannel.Name = "RadioBlueChannel";
			RadioBlueChannel.Size = new System.Drawing.Size(48, 19);
			RadioBlueChannel.TabIndex = 2;
			RadioBlueChannel.TabStop = true;
			RadioBlueChannel.Text = "Blue";
			RadioBlueChannel.UseVisualStyleBackColor = true;
			// 
			// RadioGreenChannel
			// 
			RadioGreenChannel.AutoSize = true;
			RadioGreenChannel.Location = new System.Drawing.Point(6, 31);
			RadioGreenChannel.Name = "RadioGreenChannel";
			RadioGreenChannel.Size = new System.Drawing.Size(56, 19);
			RadioGreenChannel.TabIndex = 1;
			RadioGreenChannel.TabStop = true;
			RadioGreenChannel.Text = "Green";
			RadioGreenChannel.UseVisualStyleBackColor = true;
			// 
			// RadioRedChannel
			// 
			RadioRedChannel.AutoSize = true;
			RadioRedChannel.Location = new System.Drawing.Point(6, 6);
			RadioRedChannel.Name = "RadioRedChannel";
			RadioRedChannel.Size = new System.Drawing.Size(45, 19);
			RadioRedChannel.TabIndex = 0;
			RadioRedChannel.TabStop = true;
			RadioRedChannel.Text = "Red";
			RadioRedChannel.UseVisualStyleBackColor = true;
			// 
			// TabPageBitPlanes
			// 
			TabPageBitPlanes.Controls.Add(StaticLabelBitPlaneExplanation);
			TabPageBitPlanes.Controls.Add(NUDBitPlaneIndex);
			TabPageBitPlanes.Controls.Add(StaticLabelBitPlaneIndex);
			TabPageBitPlanes.Controls.Add(RadioBitPlaneAlpha);
			TabPageBitPlanes.Controls.Add(RadioBitPlaneBlue);
			TabPageBitPlanes.Controls.Add(RadioBitPlaneGreen);
			TabPageBitPlanes.Controls.Add(RadioBitPlaneRed);
			TabPageBitPlanes.Location = new System.Drawing.Point(4, 24);
			TabPageBitPlanes.Name = "TabPageBitPlanes";
			TabPageBitPlanes.Padding = new System.Windows.Forms.Padding(3);
			TabPageBitPlanes.Size = new System.Drawing.Size(777, 106);
			TabPageBitPlanes.TabIndex = 1;
			TabPageBitPlanes.Text = "Bit Planes";
			TabPageBitPlanes.UseVisualStyleBackColor = true;
			// 
			// StaticLabelBitPlaneExplanation
			// 
			StaticLabelBitPlaneExplanation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			StaticLabelBitPlaneExplanation.AutoSize = true;
			StaticLabelBitPlaneExplanation.Location = new System.Drawing.Point(639, 81);
			StaticLabelBitPlaneExplanation.Name = "StaticLabelBitPlaneExplanation";
			StaticLabelBitPlaneExplanation.Size = new System.Drawing.Size(132, 15);
			StaticLabelBitPlaneExplanation.TabIndex = 10;
			StaticLabelBitPlaneExplanation.Text = "Requires an RGB image.";
			// 
			// NUDBitPlaneIndex
			// 
			NUDBitPlaneIndex.Location = new System.Drawing.Point(187, 6);
			NUDBitPlaneIndex.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
			NUDBitPlaneIndex.Name = "NUDBitPlaneIndex";
			NUDBitPlaneIndex.Size = new System.Drawing.Size(120, 23);
			NUDBitPlaneIndex.TabIndex = 9;
			NUDBitPlaneIndex.Value = new decimal(new int[] { 7, 0, 0, 0 });
			NUDBitPlaneIndex.ValueChanged += NUDBitPlaneIndex_ValueChanged;
			// 
			// StaticLabelBitPlaneIndex
			// 
			StaticLabelBitPlaneIndex.AutoSize = true;
			StaticLabelBitPlaneIndex.Location = new System.Drawing.Point(93, 8);
			StaticLabelBitPlaneIndex.Name = "StaticLabelBitPlaneIndex";
			StaticLabelBitPlaneIndex.Size = new System.Drawing.Size(88, 15);
			StaticLabelBitPlaneIndex.TabIndex = 8;
			StaticLabelBitPlaneIndex.Text = "Bit plane index:";
			// 
			// RadioBitPlaneAlpha
			// 
			RadioBitPlaneAlpha.AutoSize = true;
			RadioBitPlaneAlpha.Location = new System.Drawing.Point(6, 81);
			RadioBitPlaneAlpha.Name = "RadioBitPlaneAlpha";
			RadioBitPlaneAlpha.Size = new System.Drawing.Size(56, 19);
			RadioBitPlaneAlpha.TabIndex = 7;
			RadioBitPlaneAlpha.Text = "Alpha";
			RadioBitPlaneAlpha.UseVisualStyleBackColor = true;
			RadioBitPlaneAlpha.CheckedChanged += RadioBitPlaneAlpha_CheckedChanged;
			// 
			// RadioBitPlaneBlue
			// 
			RadioBitPlaneBlue.AutoSize = true;
			RadioBitPlaneBlue.Location = new System.Drawing.Point(6, 56);
			RadioBitPlaneBlue.Name = "RadioBitPlaneBlue";
			RadioBitPlaneBlue.Size = new System.Drawing.Size(48, 19);
			RadioBitPlaneBlue.TabIndex = 6;
			RadioBitPlaneBlue.Text = "Blue";
			RadioBitPlaneBlue.UseVisualStyleBackColor = true;
			RadioBitPlaneBlue.CheckedChanged += RadioBitPlaneBlue_CheckedChanged;
			// 
			// RadioBitPlaneGreen
			// 
			RadioBitPlaneGreen.AutoSize = true;
			RadioBitPlaneGreen.Location = new System.Drawing.Point(6, 31);
			RadioBitPlaneGreen.Name = "RadioBitPlaneGreen";
			RadioBitPlaneGreen.Size = new System.Drawing.Size(56, 19);
			RadioBitPlaneGreen.TabIndex = 5;
			RadioBitPlaneGreen.Text = "Green";
			RadioBitPlaneGreen.UseVisualStyleBackColor = true;
			RadioBitPlaneGreen.CheckedChanged += RadioBitPlaneGreen_CheckedChanged;
			// 
			// RadioBitPlaneRed
			// 
			RadioBitPlaneRed.AutoSize = true;
			RadioBitPlaneRed.Checked = true;
			RadioBitPlaneRed.Location = new System.Drawing.Point(6, 6);
			RadioBitPlaneRed.Name = "RadioBitPlaneRed";
			RadioBitPlaneRed.Size = new System.Drawing.Size(45, 19);
			RadioBitPlaneRed.TabIndex = 4;
			RadioBitPlaneRed.TabStop = true;
			RadioBitPlaneRed.Text = "Red";
			RadioBitPlaneRed.UseVisualStyleBackColor = true;
			RadioBitPlaneRed.CheckedChanged += RadioBitPlaneRed_CheckedChanged;
			// 
			// TabPageChromaSubsampling
			// 
			TabPageChromaSubsampling.Controls.Add(StaticLabelChromaSubsamplingExplanation);
			TabPageChromaSubsampling.Controls.Add(ButtonChromaSubsamplingSet);
			TabPageChromaSubsampling.Controls.Add(Radio411);
			TabPageChromaSubsampling.Controls.Add(Radio420);
			TabPageChromaSubsampling.Controls.Add(Radio422);
			TabPageChromaSubsampling.Controls.Add(Radio444);
			TabPageChromaSubsampling.Location = new System.Drawing.Point(4, 24);
			TabPageChromaSubsampling.Name = "TabPageChromaSubsampling";
			TabPageChromaSubsampling.Size = new System.Drawing.Size(777, 106);
			TabPageChromaSubsampling.TabIndex = 2;
			TabPageChromaSubsampling.Text = "Chroma Subsampling";
			TabPageChromaSubsampling.UseVisualStyleBackColor = true;
			// 
			// StaticLabelChromaSubsamplingExplanation
			// 
			StaticLabelChromaSubsamplingExplanation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			StaticLabelChromaSubsamplingExplanation.AutoSize = true;
			StaticLabelChromaSubsamplingExplanation.Location = new System.Drawing.Point(553, 81);
			StaticLabelChromaSubsamplingExplanation.Name = "StaticLabelChromaSubsamplingExplanation";
			StaticLabelChromaSubsamplingExplanation.Size = new System.Drawing.Size(137, 15);
			StaticLabelChromaSubsamplingExplanation.TabIndex = 13;
			StaticLabelChromaSubsamplingExplanation.Text = "Requires a YCbCr image.";
			// 
			// ButtonChromaSubsamplingSet
			// 
			ButtonChromaSubsamplingSet.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonChromaSubsamplingSet.Location = new System.Drawing.Point(696, 77);
			ButtonChromaSubsamplingSet.Name = "ButtonChromaSubsamplingSet";
			ButtonChromaSubsamplingSet.Size = new System.Drawing.Size(75, 23);
			ButtonChromaSubsamplingSet.TabIndex = 12;
			ButtonChromaSubsamplingSet.Text = "Set";
			ButtonChromaSubsamplingSet.UseVisualStyleBackColor = true;
			ButtonChromaSubsamplingSet.Click += ButtonChromaSubsamplingSet_Click;
			// 
			// Radio411
			// 
			Radio411.AutoSize = true;
			Radio411.Location = new System.Drawing.Point(6, 81);
			Radio411.Name = "Radio411";
			Radio411.Size = new System.Drawing.Size(173, 19);
			Radio411.TabIndex = 11;
			Radio411.TabStop = true;
			Radio411.Text = "4:1:1 (4x1 blocks of chroma)";
			Radio411.UseVisualStyleBackColor = true;
			// 
			// Radio420
			// 
			Radio420.AutoSize = true;
			Radio420.Location = new System.Drawing.Point(6, 56);
			Radio420.Name = "Radio420";
			Radio420.Size = new System.Drawing.Size(173, 19);
			Radio420.TabIndex = 10;
			Radio420.TabStop = true;
			Radio420.Text = "4:2:0 (2x2 blocks of chroma)";
			Radio420.UseVisualStyleBackColor = true;
			// 
			// Radio422
			// 
			Radio422.AutoSize = true;
			Radio422.Location = new System.Drawing.Point(6, 31);
			Radio422.Name = "Radio422";
			Radio422.Size = new System.Drawing.Size(173, 19);
			Radio422.TabIndex = 9;
			Radio422.TabStop = true;
			Radio422.Text = "4:2:2 (2x1 blocks of chroma)";
			Radio422.UseVisualStyleBackColor = true;
			// 
			// Radio444
			// 
			Radio444.AutoSize = true;
			Radio444.Location = new System.Drawing.Point(6, 6);
			Radio444.Name = "Radio444";
			Radio444.Size = new System.Drawing.Size(177, 19);
			Radio444.TabIndex = 8;
			Radio444.TabStop = true;
			Radio444.Text = "4:4:4 (full resolution chroma)";
			Radio444.UseVisualStyleBackColor = true;
			// 
			// TabPageReduceBitDepth
			// 
			TabPageReduceBitDepth.Controls.Add(StaticLabelReduceBitDepthExplanation);
			TabPageReduceBitDepth.Controls.Add(ButtonReduceBitDepth);
			TabPageReduceBitDepth.Controls.Add(Radio1BPPGrayscale);
			TabPageReduceBitDepth.Controls.Add(Radio2BPPTop4);
			TabPageReduceBitDepth.Controls.Add(Radio1BPPTop2);
			TabPageReduceBitDepth.Controls.Add(Radio2BPPGrayscale);
			TabPageReduceBitDepth.Controls.Add(Radio4BPPGrayscale);
			TabPageReduceBitDepth.Controls.Add(Radio8BPPGrayscale);
			TabPageReduceBitDepth.Controls.Add(Radio4BPPTop16);
			TabPageReduceBitDepth.Controls.Add(Radio4bppRGB121);
			TabPageReduceBitDepth.Controls.Add(Radio8BPPTop256);
			TabPageReduceBitDepth.Controls.Add(Radio8bppRGB332);
			TabPageReduceBitDepth.Controls.Add(Radio16bppTop65536);
			TabPageReduceBitDepth.Controls.Add(Radio16BPPRGB565);
			TabPageReduceBitDepth.Location = new System.Drawing.Point(4, 24);
			TabPageReduceBitDepth.Name = "TabPageReduceBitDepth";
			TabPageReduceBitDepth.Size = new System.Drawing.Size(777, 106);
			TabPageReduceBitDepth.TabIndex = 3;
			TabPageReduceBitDepth.Text = "Reduce Bit Depth";
			TabPageReduceBitDepth.UseVisualStyleBackColor = true;
			// 
			// StaticLabelReduceBitDepthExplanation
			// 
			StaticLabelReduceBitDepthExplanation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			StaticLabelReduceBitDepthExplanation.AutoSize = true;
			StaticLabelReduceBitDepthExplanation.Location = new System.Drawing.Point(561, 81);
			StaticLabelReduceBitDepthExplanation.Name = "StaticLabelReduceBitDepthExplanation";
			StaticLabelReduceBitDepthExplanation.Size = new System.Drawing.Size(132, 15);
			StaticLabelReduceBitDepthExplanation.TabIndex = 25;
			StaticLabelReduceBitDepthExplanation.Text = "Requires an RGB image.";
			// 
			// ButtonReduceBitDepth
			// 
			ButtonReduceBitDepth.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonReduceBitDepth.Location = new System.Drawing.Point(699, 77);
			ButtonReduceBitDepth.Name = "ButtonReduceBitDepth";
			ButtonReduceBitDepth.Size = new System.Drawing.Size(75, 23);
			ButtonReduceBitDepth.TabIndex = 24;
			ButtonReduceBitDepth.Text = "Reduce";
			ButtonReduceBitDepth.UseVisualStyleBackColor = true;
			ButtonReduceBitDepth.Click += ButtonReduceBitDepth_Click;
			// 
			// Radio1BPPGrayscale
			// 
			Radio1BPPGrayscale.AutoSize = true;
			Radio1BPPGrayscale.Location = new System.Drawing.Point(302, 81);
			Radio1BPPGrayscale.Name = "Radio1BPPGrayscale";
			Radio1BPPGrayscale.Size = new System.Drawing.Size(112, 19);
			Radio1BPPGrayscale.TabIndex = 23;
			Radio1BPPGrayscale.Text = "1bpp (grayscale)";
			Radio1BPPGrayscale.UseVisualStyleBackColor = true;
			// 
			// Radio2BPPTop4
			// 
			Radio2BPPTop4.AutoSize = true;
			Radio2BPPTop4.Location = new System.Drawing.Point(302, 6);
			Radio2BPPTop4.Name = "Radio2BPPTop4";
			Radio2BPPTop4.Size = new System.Drawing.Size(90, 19);
			Radio2BPPTop4.TabIndex = 22;
			Radio2BPPTop4.Text = "2bpp (top 4)";
			Radio2BPPTop4.UseVisualStyleBackColor = true;
			// 
			// Radio1BPPTop2
			// 
			Radio1BPPTop2.AutoSize = true;
			Radio1BPPTop2.Location = new System.Drawing.Point(302, 56);
			Radio1BPPTop2.Name = "Radio1BPPTop2";
			Radio1BPPTop2.Size = new System.Drawing.Size(125, 19);
			Radio1BPPTop2.TabIndex = 21;
			Radio1BPPTop2.Text = "1bpp (top 2 colors)";
			Radio1BPPTop2.UseVisualStyleBackColor = true;
			// 
			// Radio2BPPGrayscale
			// 
			Radio2BPPGrayscale.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			Radio2BPPGrayscale.AutoSize = true;
			Radio2BPPGrayscale.Location = new System.Drawing.Point(302, 31);
			Radio2BPPGrayscale.Name = "Radio2BPPGrayscale";
			Radio2BPPGrayscale.Size = new System.Drawing.Size(112, 19);
			Radio2BPPGrayscale.TabIndex = 20;
			Radio2BPPGrayscale.Text = "2bpp (grayscale)";
			Radio2BPPGrayscale.UseVisualStyleBackColor = true;
			// 
			// Radio4BPPGrayscale
			// 
			Radio4BPPGrayscale.AutoSize = true;
			Radio4BPPGrayscale.Location = new System.Drawing.Point(170, 81);
			Radio4BPPGrayscale.Name = "Radio4BPPGrayscale";
			Radio4BPPGrayscale.Size = new System.Drawing.Size(112, 19);
			Radio4BPPGrayscale.TabIndex = 19;
			Radio4BPPGrayscale.Text = "4bpp (grayscale)";
			Radio4BPPGrayscale.UseVisualStyleBackColor = true;
			// 
			// Radio8BPPGrayscale
			// 
			Radio8BPPGrayscale.AutoSize = true;
			Radio8BPPGrayscale.Location = new System.Drawing.Point(170, 6);
			Radio8BPPGrayscale.Name = "Radio8BPPGrayscale";
			Radio8BPPGrayscale.Size = new System.Drawing.Size(112, 19);
			Radio8BPPGrayscale.TabIndex = 18;
			Radio8BPPGrayscale.Text = "8bpp (grayscale)";
			Radio8BPPGrayscale.UseVisualStyleBackColor = true;
			// 
			// Radio4BPPTop16
			// 
			Radio4BPPTop16.AutoSize = true;
			Radio4BPPTop16.Location = new System.Drawing.Point(170, 56);
			Radio4BPPTop16.Name = "Radio4BPPTop16";
			Radio4BPPTop16.Size = new System.Drawing.Size(131, 19);
			Radio4BPPTop16.TabIndex = 17;
			Radio4BPPTop16.Text = "4bpp (top 16 colors)";
			Radio4BPPTop16.UseVisualStyleBackColor = true;
			// 
			// Radio4bppRGB121
			// 
			Radio4bppRGB121.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			Radio4bppRGB121.AutoSize = true;
			Radio4bppRGB121.Location = new System.Drawing.Point(170, 31);
			Radio4bppRGB121.Name = "Radio4bppRGB121";
			Radio4bppRGB121.Size = new System.Drawing.Size(112, 19);
			Radio4bppRGB121.TabIndex = 16;
			Radio4bppRGB121.Text = "4bpp (RGB 1:2:1)";
			Radio4bppRGB121.UseVisualStyleBackColor = true;
			// 
			// Radio8BPPTop256
			// 
			Radio8BPPTop256.AutoSize = true;
			Radio8BPPTop256.Location = new System.Drawing.Point(6, 81);
			Radio8BPPTop256.Name = "Radio8BPPTop256";
			Radio8BPPTop256.Size = new System.Drawing.Size(137, 19);
			Radio8BPPTop256.TabIndex = 15;
			Radio8BPPTop256.Text = "8bpp (top 256 colors)";
			Radio8BPPTop256.UseVisualStyleBackColor = true;
			// 
			// Radio8bppRGB332
			// 
			Radio8bppRGB332.AutoSize = true;
			Radio8bppRGB332.Location = new System.Drawing.Point(6, 56);
			Radio8bppRGB332.Name = "Radio8bppRGB332";
			Radio8bppRGB332.Size = new System.Drawing.Size(112, 19);
			Radio8bppRGB332.TabIndex = 14;
			Radio8bppRGB332.Text = "8bpp (RGB 3:3:2)";
			Radio8bppRGB332.UseVisualStyleBackColor = true;
			// 
			// Radio16bppTop65536
			// 
			Radio16bppTop65536.AutoSize = true;
			Radio16bppTop65536.Location = new System.Drawing.Point(6, 31);
			Radio16bppTop65536.Name = "Radio16bppTop65536";
			Radio16bppTop65536.Size = new System.Drawing.Size(158, 19);
			Radio16bppTop65536.TabIndex = 13;
			Radio16bppTop65536.Text = "16bpp (top 65,536 colors)";
			Radio16bppTop65536.UseVisualStyleBackColor = true;
			// 
			// Radio16BPPRGB565
			// 
			Radio16BPPRGB565.AutoSize = true;
			Radio16BPPRGB565.Checked = true;
			Radio16BPPRGB565.Location = new System.Drawing.Point(6, 6);
			Radio16BPPRGB565.Name = "Radio16BPPRGB565";
			Radio16BPPRGB565.Size = new System.Drawing.Size(118, 19);
			Radio16BPPRGB565.TabIndex = 12;
			Radio16BPPRGB565.TabStop = true;
			Radio16BPPRGB565.Text = "16bpp (RGB 5:6:5)";
			Radio16BPPRGB565.UseVisualStyleBackColor = true;
			// 
			// TabPageAnalogSignal
			// 
			TabPageAnalogSignal.Controls.Add(ButtonSaveSimplerAudio);
			TabPageAnalogSignal.Controls.Add(ButtonSaveNTSCAudio);
			TabPageAnalogSignal.Controls.Add(LabelAnalogSignalStats);
			TabPageAnalogSignal.Location = new System.Drawing.Point(4, 24);
			TabPageAnalogSignal.Name = "TabPageAnalogSignal";
			TabPageAnalogSignal.Size = new System.Drawing.Size(777, 106);
			TabPageAnalogSignal.TabIndex = 4;
			TabPageAnalogSignal.Text = "Analog Signal";
			TabPageAnalogSignal.UseVisualStyleBackColor = true;
			// 
			// ButtonSaveSimplerAudio
			// 
			ButtonSaveSimplerAudio.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonSaveSimplerAudio.Location = new System.Drawing.Point(608, 48);
			ButtonSaveSimplerAudio.Name = "ButtonSaveSimplerAudio";
			ButtonSaveSimplerAudio.Size = new System.Drawing.Size(163, 23);
			ButtonSaveSimplerAudio.TabIndex = 3;
			ButtonSaveSimplerAudio.Text = "Save simpler audio as...";
			ButtonSaveSimplerAudio.UseVisualStyleBackColor = true;
			// 
			// ButtonSaveNTSCAudio
			// 
			ButtonSaveNTSCAudio.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			ButtonSaveNTSCAudio.Location = new System.Drawing.Point(608, 77);
			ButtonSaveNTSCAudio.Name = "ButtonSaveNTSCAudio";
			ButtonSaveNTSCAudio.Size = new System.Drawing.Size(163, 23);
			ButtonSaveNTSCAudio.TabIndex = 2;
			ButtonSaveNTSCAudio.Text = "Save mock-NTSC audio as...";
			ButtonSaveNTSCAudio.UseVisualStyleBackColor = true;
			// 
			// LabelAnalogSignalStats
			// 
			LabelAnalogSignalStats.AutoSize = true;
			LabelAnalogSignalStats.Location = new System.Drawing.Point(6, 6);
			LabelAnalogSignalStats.Name = "LabelAnalogSignalStats";
			LabelAnalogSignalStats.Size = new System.Drawing.Size(121, 15);
			LabelAnalogSignalStats.TabIndex = 0;
			LabelAnalogSignalStats.Text = "Please load an image.";
			// 
			// TSMain
			// 
			TSMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TSBOpenImage, TSBSaveAs, TSSeparator1, TSBConvertToRGB, TSBConvertToYCbCr, TSBUseCurrent });
			TSMain.Location = new System.Drawing.Point(0, 0);
			TSMain.Name = "TSMain";
			TSMain.Size = new System.Drawing.Size(809, 25);
			TSMain.TabIndex = 1;
			TSMain.Text = "toolStrip1";
			// 
			// TSBOpenImage
			// 
			TSBOpenImage.Image = (System.Drawing.Image)resources.GetObject("TSBOpenImage.Image");
			TSBOpenImage.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBOpenImage.Name = "TSBOpenImage";
			TSBOpenImage.Size = new System.Drawing.Size(101, 22);
			TSBOpenImage.Text = "Open Image...";
			TSBOpenImage.Click += TSBOpenImage_Click;
			// 
			// TSBSaveAs
			// 
			TSBSaveAs.Image = (System.Drawing.Image)resources.GetObject("TSBSaveAs.Image");
			TSBSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBSaveAs.Name = "TSBSaveAs";
			TSBSaveAs.Size = new System.Drawing.Size(73, 22);
			TSBSaveAs.Text = "Save As..";
			// 
			// TSSeparator1
			// 
			TSSeparator1.Name = "TSSeparator1";
			TSSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// TSBConvertToRGB
			// 
			TSBConvertToRGB.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBConvertToRGB.Image = (System.Drawing.Image)resources.GetObject("TSBConvertToRGB.Image");
			TSBConvertToRGB.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBConvertToRGB.Name = "TSBConvertToRGB";
			TSBConvertToRGB.Size = new System.Drawing.Size(97, 22);
			TSBConvertToRGB.Text = "View RGB Image";
			TSBConvertToRGB.Click += TSBConvertToRGB_Click;
			// 
			// TSBConvertToYCbCr
			// 
			TSBConvertToYCbCr.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBConvertToYCbCr.Image = (System.Drawing.Image)resources.GetObject("TSBConvertToYCbCr.Image");
			TSBConvertToYCbCr.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBConvertToYCbCr.Name = "TSBConvertToYCbCr";
			TSBConvertToYCbCr.Size = new System.Drawing.Size(109, 22);
			TSBConvertToYCbCr.Text = "View YCbCr Image";
			TSBConvertToYCbCr.Click += TSBConvertToYCbCr_Click;
			// 
			// TSBUseCurrent
			// 
			TSBUseCurrent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			TSBUseCurrent.Image = (System.Drawing.Image)resources.GetObject("TSBUseCurrent.Image");
			TSBUseCurrent.ImageTransparentColor = System.Drawing.Color.Magenta;
			TSBUseCurrent.Name = "TSBUseCurrent";
			TSBUseCurrent.Size = new System.Drawing.Size(148, 22);
			TSBUseCurrent.Text = "Use Current as RGB Image";
			TSBUseCurrent.Click += TSBUseCurrent_Click;
			// 
			// StatusMain
			// 
			StatusMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TSSMain });
			StatusMain.Location = new System.Drawing.Point(0, 526);
			StatusMain.Name = "StatusMain";
			StatusMain.Size = new System.Drawing.Size(809, 22);
			StatusMain.TabIndex = 2;
			StatusMain.Text = "statusStrip1";
			// 
			// TSSMain
			// 
			TSSMain.Name = "TSSMain";
			TSSMain.Size = new System.Drawing.Size(276, 17);
			TSSMain.Text = "file.jpg | 0x0 | Viewing RGB image (YCbCr available)";
			// 
			// panel1
			// 
			panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			panel1.AutoScroll = true;
			panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			panel1.Controls.Add(PictureMain);
			panel1.Location = new System.Drawing.Point(16, 168);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(781, 355);
			panel1.TabIndex = 3;
			// 
			// PictureMain
			// 
			PictureMain.Location = new System.Drawing.Point(4, 3);
			PictureMain.Name = "PictureMain";
			PictureMain.Size = new System.Drawing.Size(100, 50);
			PictureMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			PictureMain.TabIndex = 0;
			PictureMain.TabStop = false;
			// 
			// OFDOpenImage
			// 
			OFDOpenImage.Filter = "All files|*.*";
			OFDOpenImage.Title = "ByteView - Open Image";
			// 
			// SFDSaveImage
			// 
			SFDSaveImage.Filter = "Portable Networked Graphics|*.png|All files|*.*";
			SFDSaveImage.Title = "ByteView - Save Image";
			// 
			// SFDSaveAnalogSignal
			// 
			SFDSaveAnalogSignal.DefaultExt = "raw";
			SFDSaveAnalogSignal.Filter = "Raw Audio (mono, 48kHz, single-precision float)|*.raw|All files|*.*";
			SFDSaveAnalogSignal.Title = "ByteView - Save Analog Signal";
			// 
			// RadioValueChannel
			// 
			RadioValueChannel.AutoSize = true;
			RadioValueChannel.Location = new System.Drawing.Point(220, 56);
			RadioValueChannel.Name = "RadioValueChannel";
			RadioValueChannel.Size = new System.Drawing.Size(53, 19);
			RadioValueChannel.TabIndex = 12;
			RadioValueChannel.TabStop = true;
			RadioValueChannel.Text = "Value";
			RadioValueChannel.UseVisualStyleBackColor = true;
			// 
			// RadioSaturationChannel
			// 
			RadioSaturationChannel.AutoSize = true;
			RadioSaturationChannel.Location = new System.Drawing.Point(220, 31);
			RadioSaturationChannel.Name = "RadioSaturationChannel";
			RadioSaturationChannel.Size = new System.Drawing.Size(79, 19);
			RadioSaturationChannel.TabIndex = 11;
			RadioSaturationChannel.TabStop = true;
			RadioSaturationChannel.Text = "Saturation";
			RadioSaturationChannel.UseVisualStyleBackColor = true;
			// 
			// RadioHueChannel
			// 
			RadioHueChannel.AutoSize = true;
			RadioHueChannel.Location = new System.Drawing.Point(220, 6);
			RadioHueChannel.Name = "RadioHueChannel";
			RadioHueChannel.Size = new System.Drawing.Size(47, 19);
			RadioHueChannel.TabIndex = 10;
			RadioHueChannel.TabStop = true;
			RadioHueChannel.Text = "Hue";
			RadioHueChannel.UseVisualStyleBackColor = true;
			// 
			// ChromaPlaygroundForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(809, 548);
			Controls.Add(panel1);
			Controls.Add(StatusMain);
			Controls.Add(TSMain);
			Controls.Add(TabsOptions);
			Name = "ChromaPlaygroundForm";
			Text = "Chroma Playground";
			TabsOptions.ResumeLayout(false);
			TabPageColorChannels.ResumeLayout(false);
			TabPageColorChannels.PerformLayout();
			TabPageBitPlanes.ResumeLayout(false);
			TabPageBitPlanes.PerformLayout();
			((System.ComponentModel.ISupportInitialize)NUDBitPlaneIndex).EndInit();
			TabPageChromaSubsampling.ResumeLayout(false);
			TabPageChromaSubsampling.PerformLayout();
			TabPageReduceBitDepth.ResumeLayout(false);
			TabPageReduceBitDepth.PerformLayout();
			TabPageAnalogSignal.ResumeLayout(false);
			TabPageAnalogSignal.PerformLayout();
			TSMain.ResumeLayout(false);
			TSMain.PerformLayout();
			StatusMain.ResumeLayout(false);
			StatusMain.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)PictureMain).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.TabControl TabsOptions;
		private System.Windows.Forms.TabPage TabPageColorChannels;
		private System.Windows.Forms.TabPage TabPageBitPlanes;
		private System.Windows.Forms.TabPage TabPageChromaSubsampling;
		private System.Windows.Forms.TabPage TabPageReduceBitDepth;
		private System.Windows.Forms.TabPage TabPageAnalogSignal;
		private System.Windows.Forms.ToolStrip TSMain;
		private System.Windows.Forms.ToolStripButton TSBOpenImage;
		private System.Windows.Forms.RadioButton RadioAlphaChannel;
		private System.Windows.Forms.RadioButton RadioBlueChannel;
		private System.Windows.Forms.RadioButton RadioGreenChannel;
		private System.Windows.Forms.RadioButton RadioRedChannel;
		private System.Windows.Forms.ToolStripButton TSBSaveAs;
		private System.Windows.Forms.ToolStripSeparator TSSeparator1;
		private System.Windows.Forms.ToolStripButton TSBConvertToRGB;
		private System.Windows.Forms.RadioButton RadioChrominance;
		private System.Windows.Forms.RadioButton RadioCr;
		private System.Windows.Forms.RadioButton RadioCb;
		private System.Windows.Forms.RadioButton RadioLuminance;
		private System.Windows.Forms.Button ButtonSetColorChannel;
		private System.Windows.Forms.NumericUpDown NUDBitPlaneIndex;
		private System.Windows.Forms.Label StaticLabelBitPlaneIndex;
		private System.Windows.Forms.RadioButton RadioBitPlaneAlpha;
		private System.Windows.Forms.RadioButton RadioBitPlaneBlue;
		private System.Windows.Forms.RadioButton RadioBitPlaneGreen;
		private System.Windows.Forms.RadioButton RadioBitPlaneRed;
		private System.Windows.Forms.Label StaticLabelBitPlaneExplanation;
		private System.Windows.Forms.RadioButton Radio411;
		private System.Windows.Forms.RadioButton Radio420;
		private System.Windows.Forms.RadioButton Radio422;
		private System.Windows.Forms.RadioButton Radio444;
		private System.Windows.Forms.Label StaticLabelChromaSubsamplingExplanation;
		private System.Windows.Forms.Button ButtonChromaSubsamplingSet;
		private System.Windows.Forms.ToolStripButton TSBConvertToYCbCr;
		private System.Windows.Forms.RadioButton Radio1BPPGrayscale;
		private System.Windows.Forms.RadioButton Radio2BPPTop4;
		private System.Windows.Forms.RadioButton Radio1BPPTop2;
		private System.Windows.Forms.RadioButton Radio2BPPGrayscale;
		private System.Windows.Forms.RadioButton Radio4BPPGrayscale;
		private System.Windows.Forms.RadioButton Radio8BPPGrayscale;
		private System.Windows.Forms.RadioButton Radio4BPPTop16;
		private System.Windows.Forms.RadioButton Radio4bppRGB121;
		private System.Windows.Forms.RadioButton Radio8BPPTop256;
		private System.Windows.Forms.RadioButton Radio8bppRGB332;
		private System.Windows.Forms.RadioButton Radio16bppTop65536;
		private System.Windows.Forms.RadioButton Radio16BPPRGB565;
		private System.Windows.Forms.Label StaticLabelReduceBitDepthExplanation;
		private System.Windows.Forms.Button ButtonReduceBitDepth;
		private System.Windows.Forms.Button ButtonSaveNTSCAudio;
		private System.Windows.Forms.Label LabelAnalogSignalStats;
		private System.Windows.Forms.StatusStrip StatusMain;
		private System.Windows.Forms.ToolStripStatusLabel TSSMain;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox PictureMain;
		private System.Windows.Forms.OpenFileDialog OFDOpenImage;
		private System.Windows.Forms.SaveFileDialog SFDSaveImage;
		private System.Windows.Forms.SaveFileDialog SFDSaveAnalogSignal;
		private System.Windows.Forms.Button ButtonSaveSimplerAudio;
		private System.Windows.Forms.Button ButtonColorChannelReset;
		private System.Windows.Forms.ToolStripButton TSBUseCurrent;
		private System.Windows.Forms.RadioButton RadioValueChannel;
		private System.Windows.Forms.RadioButton RadioSaturationChannel;
		private System.Windows.Forms.RadioButton RadioHueChannel;
	}
}