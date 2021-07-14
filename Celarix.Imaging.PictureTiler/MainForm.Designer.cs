namespace Celarix.Imaging.PictureTiler
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
            this.TabsMain = new System.Windows.Forms.TabControl();
            this.TabTiler = new System.Windows.Forms.TabPage();
            this.ButtonTilerCancel = new System.Windows.Forms.Button();
            this.LabelTilerStatus = new System.Windows.Forms.Label();
            this.ProgressTiler = new System.Windows.Forms.ProgressBar();
            this.GroupTilerOptions = new System.Windows.Forms.GroupBox();
            this.StaticLabelTilerOutputPath = new System.Windows.Forms.Label();
            this.ButtonTilerGenerate = new System.Windows.Forms.Button();
            this.StaticLabelTilerInputPath = new System.Windows.Forms.Label();
            this.NUDTilerTileHeight = new System.Windows.Forms.NumericUpDown();
            this.TextTilerInputFolder = new System.Windows.Forms.TextBox();
            this.StaticLabelTilerTileHeight = new System.Windows.Forms.Label();
            this.ButtonTilerSelectInputFolder = new System.Windows.Forms.Button();
            this.NUDTilerTileWidth = new System.Windows.Forms.NumericUpDown();
            this.TextTilerOutputPath = new System.Windows.Forms.TextBox();
            this.StaticLabelTilerTileWidth = new System.Windows.Forms.Label();
            this.ButtonTilerSelectOutputPath = new System.Windows.Forms.Button();
            this.TabPacker = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CheckPackerMultipicture = new System.Windows.Forms.CheckBox();
            this.CheckPackerRecursive = new System.Windows.Forms.CheckBox();
            this.StaticLabelPackerOutputPath = new System.Windows.Forms.Label();
            this.ButtonPackerGenerate = new System.Windows.Forms.Button();
            this.StaticLabelPackerInputPath = new System.Windows.Forms.Label();
            this.TextPackerInputPath = new System.Windows.Forms.TextBox();
            this.ButtonPackerSelectInputPath = new System.Windows.Forms.Button();
            this.TextPackerOutputPath = new System.Windows.Forms.TextBox();
            this.ButtonPackerSelectOutputPath = new System.Windows.Forms.Button();
            this.ProgressPacker = new System.Windows.Forms.ProgressBar();
            this.ButtonPackerCancel = new System.Windows.Forms.Button();
            this.LabelPackerStatus = new System.Windows.Forms.Label();
            this.TabCanvas = new System.Windows.Forms.TabPage();
            this.GroupCanvas = new System.Windows.Forms.GroupBox();
            this.StaticLabelCanvasOutputPath = new System.Windows.Forms.Label();
            this.ButtonCanvasGenerate = new System.Windows.Forms.Button();
            this.StaticLabelCanvasInputPath = new System.Windows.Forms.Label();
            this.TextCanvasInputPath = new System.Windows.Forms.TextBox();
            this.ButtonCanvasSelectInputPath = new System.Windows.Forms.Button();
            this.TextCanvasOutputPath = new System.Windows.Forms.TextBox();
            this.ButtonCanvasSelectOutputPath = new System.Windows.Forms.Button();
            this.LabelCanvasStatus = new System.Windows.Forms.Label();
            this.ButtonCanvasCancel = new System.Windows.Forms.Button();
            this.FBDMain = new System.Windows.Forms.FolderBrowserDialog();
            this.SFDMain = new System.Windows.Forms.SaveFileDialog();
            this.OFDMain = new System.Windows.Forms.OpenFileDialog();
            this.CheckTreatInputAsPathsFile = new System.Windows.Forms.CheckBox();
            this.TabsMain.SuspendLayout();
            this.TabTiler.SuspendLayout();
            this.GroupTilerOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDTilerTileHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDTilerTileWidth)).BeginInit();
            this.TabPacker.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.TabCanvas.SuspendLayout();
            this.GroupCanvas.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabsMain
            // 
            this.TabsMain.Controls.Add(this.TabTiler);
            this.TabsMain.Controls.Add(this.TabPacker);
            this.TabsMain.Controls.Add(this.TabCanvas);
            this.TabsMain.Location = new System.Drawing.Point(8, 7);
            this.TabsMain.Margin = new System.Windows.Forms.Padding(2);
            this.TabsMain.Name = "TabsMain";
            this.TabsMain.SelectedIndex = 1;
            this.TabsMain.Size = new System.Drawing.Size(348, 392);
            this.TabsMain.TabIndex = 0;
            // 
            // TabTiler
            // 
            this.TabTiler.Controls.Add(this.ButtonTilerCancel);
            this.TabTiler.Controls.Add(this.LabelTilerStatus);
            this.TabTiler.Controls.Add(this.ProgressTiler);
            this.TabTiler.Controls.Add(this.GroupTilerOptions);
            this.TabTiler.Location = new System.Drawing.Point(4, 24);
            this.TabTiler.Margin = new System.Windows.Forms.Padding(2);
            this.TabTiler.Name = "TabTiler";
            this.TabTiler.Padding = new System.Windows.Forms.Padding(2);
            this.TabTiler.Size = new System.Drawing.Size(340, 364);
            this.TabTiler.TabIndex = 0;
            this.TabTiler.Text = "Tiler";
            this.TabTiler.UseVisualStyleBackColor = true;
            // 
            // ButtonTilerCancel
            // 
            this.ButtonTilerCancel.Enabled = false;
            this.ButtonTilerCancel.Location = new System.Drawing.Point(256, 340);
            this.ButtonTilerCancel.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonTilerCancel.Name = "ButtonTilerCancel";
            this.ButtonTilerCancel.Size = new System.Drawing.Size(78, 20);
            this.ButtonTilerCancel.TabIndex = 9;
            this.ButtonTilerCancel.Text = "&Cancel";
            this.ButtonTilerCancel.UseVisualStyleBackColor = true;
            this.ButtonTilerCancel.Click += new System.EventHandler(this.ButtonTilerCancel_Click);
            // 
            // LabelTilerStatus
            // 
            this.LabelTilerStatus.AutoSize = true;
            this.LabelTilerStatus.Location = new System.Drawing.Point(9, 160);
            this.LabelTilerStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelTilerStatus.Name = "LabelTilerStatus";
            this.LabelTilerStatus.Size = new System.Drawing.Size(57, 15);
            this.LabelTilerStatus.TabIndex = 8;
            this.LabelTilerStatus.Text = "Waiting...";
            // 
            // ProgressTiler
            // 
            this.ProgressTiler.Location = new System.Drawing.Point(9, 130);
            this.ProgressTiler.Margin = new System.Windows.Forms.Padding(2);
            this.ProgressTiler.Name = "ProgressTiler";
            this.ProgressTiler.Size = new System.Drawing.Size(325, 20);
            this.ProgressTiler.TabIndex = 7;
            // 
            // GroupTilerOptions
            // 
            this.GroupTilerOptions.Controls.Add(this.StaticLabelTilerOutputPath);
            this.GroupTilerOptions.Controls.Add(this.ButtonTilerGenerate);
            this.GroupTilerOptions.Controls.Add(this.StaticLabelTilerInputPath);
            this.GroupTilerOptions.Controls.Add(this.NUDTilerTileHeight);
            this.GroupTilerOptions.Controls.Add(this.TextTilerInputFolder);
            this.GroupTilerOptions.Controls.Add(this.StaticLabelTilerTileHeight);
            this.GroupTilerOptions.Controls.Add(this.ButtonTilerSelectInputFolder);
            this.GroupTilerOptions.Controls.Add(this.NUDTilerTileWidth);
            this.GroupTilerOptions.Controls.Add(this.TextTilerOutputPath);
            this.GroupTilerOptions.Controls.Add(this.StaticLabelTilerTileWidth);
            this.GroupTilerOptions.Controls.Add(this.ButtonTilerSelectOutputPath);
            this.GroupTilerOptions.Location = new System.Drawing.Point(9, 10);
            this.GroupTilerOptions.Margin = new System.Windows.Forms.Padding(2);
            this.GroupTilerOptions.Name = "GroupTilerOptions";
            this.GroupTilerOptions.Padding = new System.Windows.Forms.Padding(2);
            this.GroupTilerOptions.Size = new System.Drawing.Size(325, 116);
            this.GroupTilerOptions.TabIndex = 6;
            this.GroupTilerOptions.TabStop = false;
            this.GroupTilerOptions.Text = "Options";
            // 
            // StaticLabelTilerOutputPath
            // 
            this.StaticLabelTilerOutputPath.AutoSize = true;
            this.StaticLabelTilerOutputPath.Location = new System.Drawing.Point(4, 37);
            this.StaticLabelTilerOutputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelTilerOutputPath.Name = "StaticLabelTilerOutputPath";
            this.StaticLabelTilerOutputPath.Size = new System.Drawing.Size(84, 15);
            this.StaticLabelTilerOutputPath.TabIndex = 0;
            this.StaticLabelTilerOutputPath.Text = "Output Image:";
            // 
            // ButtonTilerGenerate
            // 
            this.ButtonTilerGenerate.Location = new System.Drawing.Point(5, 92);
            this.ButtonTilerGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonTilerGenerate.Name = "ButtonTilerGenerate";
            this.ButtonTilerGenerate.Size = new System.Drawing.Size(316, 20);
            this.ButtonTilerGenerate.TabIndex = 5;
            this.ButtonTilerGenerate.Text = "&Generate";
            this.ButtonTilerGenerate.UseVisualStyleBackColor = true;
            this.ButtonTilerGenerate.Click += new System.EventHandler(this.ButtonTilerGenerate_Click);
            // 
            // StaticLabelTilerInputPath
            // 
            this.StaticLabelTilerInputPath.AutoSize = true;
            this.StaticLabelTilerInputPath.Location = new System.Drawing.Point(4, 15);
            this.StaticLabelTilerInputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelTilerInputPath.Name = "StaticLabelTilerInputPath";
            this.StaticLabelTilerInputPath.Size = new System.Drawing.Size(74, 15);
            this.StaticLabelTilerInputPath.TabIndex = 0;
            this.StaticLabelTilerInputPath.Text = "Input Folder:";
            // 
            // NUDTilerTileHeight
            // 
            this.NUDTilerTileHeight.Location = new System.Drawing.Point(240, 64);
            this.NUDTilerTileHeight.Margin = new System.Windows.Forms.Padding(2);
            this.NUDTilerTileHeight.Name = "NUDTilerTileHeight";
            this.NUDTilerTileHeight.Size = new System.Drawing.Size(80, 23);
            this.NUDTilerTileHeight.TabIndex = 4;
            // 
            // TextTilerInputFolder
            // 
            this.TextTilerInputFolder.Location = new System.Drawing.Point(88, 13);
            this.TextTilerInputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.TextTilerInputFolder.Name = "TextTilerInputFolder";
            this.TextTilerInputFolder.Size = new System.Drawing.Size(204, 23);
            this.TextTilerInputFolder.TabIndex = 1;
            // 
            // StaticLabelTilerTileHeight
            // 
            this.StaticLabelTilerTileHeight.AutoSize = true;
            this.StaticLabelTilerTileHeight.Location = new System.Drawing.Point(169, 65);
            this.StaticLabelTilerTileHeight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelTilerTileHeight.Name = "StaticLabelTilerTileHeight";
            this.StaticLabelTilerTileHeight.Size = new System.Drawing.Size(67, 15);
            this.StaticLabelTilerTileHeight.TabIndex = 3;
            this.StaticLabelTilerTileHeight.Text = "Tile Height:";
            // 
            // ButtonTilerSelectInputFolder
            // 
            this.ButtonTilerSelectInputFolder.Location = new System.Drawing.Point(292, 12);
            this.ButtonTilerSelectInputFolder.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonTilerSelectInputFolder.Name = "ButtonTilerSelectInputFolder";
            this.ButtonTilerSelectInputFolder.Size = new System.Drawing.Size(29, 20);
            this.ButtonTilerSelectInputFolder.TabIndex = 2;
            this.ButtonTilerSelectInputFolder.Text = "...";
            this.ButtonTilerSelectInputFolder.UseVisualStyleBackColor = true;
            this.ButtonTilerSelectInputFolder.Click += new System.EventHandler(this.ButtonTilerSelectInputFolder_Click);
            // 
            // NUDTilerTileWidth
            // 
            this.NUDTilerTileWidth.Location = new System.Drawing.Point(75, 64);
            this.NUDTilerTileWidth.Margin = new System.Windows.Forms.Padding(2);
            this.NUDTilerTileWidth.Name = "NUDTilerTileWidth";
            this.NUDTilerTileWidth.Size = new System.Drawing.Size(80, 23);
            this.NUDTilerTileWidth.TabIndex = 4;
            // 
            // TextTilerOutputPath
            // 
            this.TextTilerOutputPath.Location = new System.Drawing.Point(98, 35);
            this.TextTilerOutputPath.Margin = new System.Windows.Forms.Padding(2);
            this.TextTilerOutputPath.Name = "TextTilerOutputPath";
            this.TextTilerOutputPath.Size = new System.Drawing.Size(194, 23);
            this.TextTilerOutputPath.TabIndex = 1;
            // 
            // StaticLabelTilerTileWidth
            // 
            this.StaticLabelTilerTileWidth.AutoSize = true;
            this.StaticLabelTilerTileWidth.Location = new System.Drawing.Point(4, 65);
            this.StaticLabelTilerTileWidth.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelTilerTileWidth.Name = "StaticLabelTilerTileWidth";
            this.StaticLabelTilerTileWidth.Size = new System.Drawing.Size(63, 15);
            this.StaticLabelTilerTileWidth.TabIndex = 3;
            this.StaticLabelTilerTileWidth.Text = "Tile Width:";
            // 
            // ButtonTilerSelectOutputPath
            // 
            this.ButtonTilerSelectOutputPath.Location = new System.Drawing.Point(292, 34);
            this.ButtonTilerSelectOutputPath.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonTilerSelectOutputPath.Name = "ButtonTilerSelectOutputPath";
            this.ButtonTilerSelectOutputPath.Size = new System.Drawing.Size(29, 20);
            this.ButtonTilerSelectOutputPath.TabIndex = 2;
            this.ButtonTilerSelectOutputPath.Text = "...";
            this.ButtonTilerSelectOutputPath.UseVisualStyleBackColor = true;
            this.ButtonTilerSelectOutputPath.Click += new System.EventHandler(this.ButtonTilerSelectOutputPath_Click);
            // 
            // TabPacker
            // 
            this.TabPacker.Controls.Add(this.CheckTreatInputAsPathsFile);
            this.TabPacker.Controls.Add(this.groupBox1);
            this.TabPacker.Controls.Add(this.ProgressPacker);
            this.TabPacker.Controls.Add(this.ButtonPackerCancel);
            this.TabPacker.Controls.Add(this.LabelPackerStatus);
            this.TabPacker.Location = new System.Drawing.Point(4, 24);
            this.TabPacker.Margin = new System.Windows.Forms.Padding(2);
            this.TabPacker.Name = "TabPacker";
            this.TabPacker.Padding = new System.Windows.Forms.Padding(2);
            this.TabPacker.Size = new System.Drawing.Size(340, 364);
            this.TabPacker.TabIndex = 1;
            this.TabPacker.Text = "Packer";
            this.TabPacker.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CheckPackerMultipicture);
            this.groupBox1.Controls.Add(this.CheckPackerRecursive);
            this.groupBox1.Controls.Add(this.StaticLabelPackerOutputPath);
            this.groupBox1.Controls.Add(this.ButtonPackerGenerate);
            this.groupBox1.Controls.Add(this.StaticLabelPackerInputPath);
            this.groupBox1.Controls.Add(this.TextPackerInputPath);
            this.groupBox1.Controls.Add(this.ButtonPackerSelectInputPath);
            this.groupBox1.Controls.Add(this.TextPackerOutputPath);
            this.groupBox1.Controls.Add(this.ButtonPackerSelectOutputPath);
            this.groupBox1.Location = new System.Drawing.Point(8, 30);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(325, 116);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // CheckPackerMultipicture
            // 
            this.CheckPackerMultipicture.AutoSize = true;
            this.CheckPackerMultipicture.Location = new System.Drawing.Point(149, 62);
            this.CheckPackerMultipicture.Margin = new System.Windows.Forms.Padding(2);
            this.CheckPackerMultipicture.Name = "CheckPackerMultipicture";
            this.CheckPackerMultipicture.Size = new System.Drawing.Size(167, 19);
            this.CheckPackerMultipicture.TabIndex = 7;
            this.CheckPackerMultipicture.Text = "Generate &zoomable canvas";
            this.CheckPackerMultipicture.UseVisualStyleBackColor = true;
            // 
            // CheckPackerRecursive
            // 
            this.CheckPackerRecursive.AutoSize = true;
            this.CheckPackerRecursive.Location = new System.Drawing.Point(5, 62);
            this.CheckPackerRecursive.Margin = new System.Windows.Forms.Padding(2);
            this.CheckPackerRecursive.Name = "CheckPackerRecursive";
            this.CheckPackerRecursive.Size = new System.Drawing.Size(135, 19);
            this.CheckPackerRecursive.TabIndex = 6;
            this.CheckPackerRecursive.Text = "Load files &recursively";
            this.CheckPackerRecursive.UseVisualStyleBackColor = true;
            // 
            // StaticLabelPackerOutputPath
            // 
            this.StaticLabelPackerOutputPath.AutoSize = true;
            this.StaticLabelPackerOutputPath.Location = new System.Drawing.Point(4, 37);
            this.StaticLabelPackerOutputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelPackerOutputPath.Name = "StaticLabelPackerOutputPath";
            this.StaticLabelPackerOutputPath.Size = new System.Drawing.Size(84, 15);
            this.StaticLabelPackerOutputPath.TabIndex = 0;
            this.StaticLabelPackerOutputPath.Text = "Output Folder:";
            // 
            // ButtonPackerGenerate
            // 
            this.ButtonPackerGenerate.Location = new System.Drawing.Point(5, 92);
            this.ButtonPackerGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonPackerGenerate.Name = "ButtonPackerGenerate";
            this.ButtonPackerGenerate.Size = new System.Drawing.Size(316, 20);
            this.ButtonPackerGenerate.TabIndex = 5;
            this.ButtonPackerGenerate.Text = "&Generate";
            this.ButtonPackerGenerate.UseVisualStyleBackColor = true;
            this.ButtonPackerGenerate.Click += new System.EventHandler(this.ButtonPackerGenerate_Click);
            // 
            // StaticLabelPackerInputPath
            // 
            this.StaticLabelPackerInputPath.AutoSize = true;
            this.StaticLabelPackerInputPath.Location = new System.Drawing.Point(4, 15);
            this.StaticLabelPackerInputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelPackerInputPath.Name = "StaticLabelPackerInputPath";
            this.StaticLabelPackerInputPath.Size = new System.Drawing.Size(38, 15);
            this.StaticLabelPackerInputPath.TabIndex = 0;
            this.StaticLabelPackerInputPath.Text = "Input:";
            // 
            // TextPackerInputPath
            // 
            this.TextPackerInputPath.Location = new System.Drawing.Point(46, 13);
            this.TextPackerInputPath.Margin = new System.Windows.Forms.Padding(2);
            this.TextPackerInputPath.Name = "TextPackerInputPath";
            this.TextPackerInputPath.Size = new System.Drawing.Size(246, 23);
            this.TextPackerInputPath.TabIndex = 1;
            // 
            // ButtonPackerSelectInputPath
            // 
            this.ButtonPackerSelectInputPath.Location = new System.Drawing.Point(292, 12);
            this.ButtonPackerSelectInputPath.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonPackerSelectInputPath.Name = "ButtonPackerSelectInputPath";
            this.ButtonPackerSelectInputPath.Size = new System.Drawing.Size(29, 20);
            this.ButtonPackerSelectInputPath.TabIndex = 2;
            this.ButtonPackerSelectInputPath.Text = "...";
            this.ButtonPackerSelectInputPath.UseVisualStyleBackColor = true;
            this.ButtonPackerSelectInputPath.Click += new System.EventHandler(this.ButtonPackerSelectInputPath_Click);
            // 
            // TextPackerOutputPath
            // 
            this.TextPackerOutputPath.Location = new System.Drawing.Point(98, 35);
            this.TextPackerOutputPath.Margin = new System.Windows.Forms.Padding(2);
            this.TextPackerOutputPath.Name = "TextPackerOutputPath";
            this.TextPackerOutputPath.Size = new System.Drawing.Size(194, 23);
            this.TextPackerOutputPath.TabIndex = 1;
            // 
            // ButtonPackerSelectOutputPath
            // 
            this.ButtonPackerSelectOutputPath.Location = new System.Drawing.Point(292, 34);
            this.ButtonPackerSelectOutputPath.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonPackerSelectOutputPath.Name = "ButtonPackerSelectOutputPath";
            this.ButtonPackerSelectOutputPath.Size = new System.Drawing.Size(29, 20);
            this.ButtonPackerSelectOutputPath.TabIndex = 2;
            this.ButtonPackerSelectOutputPath.Text = "...";
            this.ButtonPackerSelectOutputPath.UseVisualStyleBackColor = true;
            this.ButtonPackerSelectOutputPath.Click += new System.EventHandler(this.ButtonPackerSelectOutputPath_Click);
            // 
            // ProgressPacker
            // 
            this.ProgressPacker.Location = new System.Drawing.Point(8, 133);
            this.ProgressPacker.Margin = new System.Windows.Forms.Padding(2);
            this.ProgressPacker.Name = "ProgressPacker";
            this.ProgressPacker.Size = new System.Drawing.Size(325, 20);
            this.ProgressPacker.TabIndex = 7;
            // 
            // ButtonPackerCancel
            // 
            this.ButtonPackerCancel.Enabled = false;
            this.ButtonPackerCancel.Location = new System.Drawing.Point(255, 344);
            this.ButtonPackerCancel.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonPackerCancel.Name = "ButtonPackerCancel";
            this.ButtonPackerCancel.Size = new System.Drawing.Size(78, 20);
            this.ButtonPackerCancel.TabIndex = 9;
            this.ButtonPackerCancel.Text = "&Cancel";
            this.ButtonPackerCancel.UseVisualStyleBackColor = true;
            this.ButtonPackerCancel.Click += new System.EventHandler(this.ButtonPackerCancel_Click);
            // 
            // LabelPackerStatus
            // 
            this.LabelPackerStatus.AutoSize = true;
            this.LabelPackerStatus.Location = new System.Drawing.Point(8, 180);
            this.LabelPackerStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelPackerStatus.Name = "LabelPackerStatus";
            this.LabelPackerStatus.Size = new System.Drawing.Size(57, 15);
            this.LabelPackerStatus.TabIndex = 8;
            this.LabelPackerStatus.Text = "Waiting...";
            // 
            // TabCanvas
            // 
            this.TabCanvas.BackColor = System.Drawing.Color.White;
            this.TabCanvas.Controls.Add(this.GroupCanvas);
            this.TabCanvas.Controls.Add(this.LabelCanvasStatus);
            this.TabCanvas.Controls.Add(this.ButtonCanvasCancel);
            this.TabCanvas.Location = new System.Drawing.Point(4, 24);
            this.TabCanvas.Margin = new System.Windows.Forms.Padding(2);
            this.TabCanvas.Name = "TabCanvas";
            this.TabCanvas.Size = new System.Drawing.Size(340, 364);
            this.TabCanvas.TabIndex = 2;
            this.TabCanvas.Text = "Canvas from Existing";
            // 
            // GroupCanvas
            // 
            this.GroupCanvas.Controls.Add(this.StaticLabelCanvasOutputPath);
            this.GroupCanvas.Controls.Add(this.ButtonCanvasGenerate);
            this.GroupCanvas.Controls.Add(this.StaticLabelCanvasInputPath);
            this.GroupCanvas.Controls.Add(this.TextCanvasInputPath);
            this.GroupCanvas.Controls.Add(this.ButtonCanvasSelectInputPath);
            this.GroupCanvas.Controls.Add(this.TextCanvasOutputPath);
            this.GroupCanvas.Controls.Add(this.ButtonCanvasSelectOutputPath);
            this.GroupCanvas.Location = new System.Drawing.Point(8, 10);
            this.GroupCanvas.Margin = new System.Windows.Forms.Padding(2);
            this.GroupCanvas.Name = "GroupCanvas";
            this.GroupCanvas.Padding = new System.Windows.Forms.Padding(2);
            this.GroupCanvas.Size = new System.Drawing.Size(325, 86);
            this.GroupCanvas.TabIndex = 6;
            this.GroupCanvas.TabStop = false;
            this.GroupCanvas.Text = "Options";
            // 
            // StaticLabelCanvasOutputPath
            // 
            this.StaticLabelCanvasOutputPath.AutoSize = true;
            this.StaticLabelCanvasOutputPath.Location = new System.Drawing.Point(4, 37);
            this.StaticLabelCanvasOutputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelCanvasOutputPath.Name = "StaticLabelCanvasOutputPath";
            this.StaticLabelCanvasOutputPath.Size = new System.Drawing.Size(84, 15);
            this.StaticLabelCanvasOutputPath.TabIndex = 0;
            this.StaticLabelCanvasOutputPath.Text = "Output Folder:";
            // 
            // ButtonCanvasGenerate
            // 
            this.ButtonCanvasGenerate.Location = new System.Drawing.Point(4, 58);
            this.ButtonCanvasGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCanvasGenerate.Name = "ButtonCanvasGenerate";
            this.ButtonCanvasGenerate.Size = new System.Drawing.Size(316, 20);
            this.ButtonCanvasGenerate.TabIndex = 5;
            this.ButtonCanvasGenerate.Text = "&Generate";
            this.ButtonCanvasGenerate.UseVisualStyleBackColor = true;
            this.ButtonCanvasGenerate.Click += new System.EventHandler(this.ButtonCanvasGenerate_Click);
            // 
            // StaticLabelCanvasInputPath
            // 
            this.StaticLabelCanvasInputPath.AutoSize = true;
            this.StaticLabelCanvasInputPath.Location = new System.Drawing.Point(4, 15);
            this.StaticLabelCanvasInputPath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.StaticLabelCanvasInputPath.Name = "StaticLabelCanvasInputPath";
            this.StaticLabelCanvasInputPath.Size = new System.Drawing.Size(74, 15);
            this.StaticLabelCanvasInputPath.TabIndex = 0;
            this.StaticLabelCanvasInputPath.Text = "Input Image:";
            // 
            // TextCanvasInputPath
            // 
            this.TextCanvasInputPath.Location = new System.Drawing.Point(88, 13);
            this.TextCanvasInputPath.Margin = new System.Windows.Forms.Padding(2);
            this.TextCanvasInputPath.Name = "TextCanvasInputPath";
            this.TextCanvasInputPath.Size = new System.Drawing.Size(204, 23);
            this.TextCanvasInputPath.TabIndex = 1;
            // 
            // ButtonCanvasSelectInputPath
            // 
            this.ButtonCanvasSelectInputPath.Location = new System.Drawing.Point(292, 12);
            this.ButtonCanvasSelectInputPath.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCanvasSelectInputPath.Name = "ButtonCanvasSelectInputPath";
            this.ButtonCanvasSelectInputPath.Size = new System.Drawing.Size(29, 20);
            this.ButtonCanvasSelectInputPath.TabIndex = 2;
            this.ButtonCanvasSelectInputPath.Text = "...";
            this.ButtonCanvasSelectInputPath.UseVisualStyleBackColor = true;
            this.ButtonCanvasSelectInputPath.Click += new System.EventHandler(this.ButtonCanvasSelectInputPath_Click);
            // 
            // TextCanvasOutputPath
            // 
            this.TextCanvasOutputPath.Location = new System.Drawing.Point(98, 35);
            this.TextCanvasOutputPath.Margin = new System.Windows.Forms.Padding(2);
            this.TextCanvasOutputPath.Name = "TextCanvasOutputPath";
            this.TextCanvasOutputPath.Size = new System.Drawing.Size(194, 23);
            this.TextCanvasOutputPath.TabIndex = 1;
            // 
            // ButtonCanvasSelectOutputPath
            // 
            this.ButtonCanvasSelectOutputPath.Location = new System.Drawing.Point(292, 34);
            this.ButtonCanvasSelectOutputPath.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCanvasSelectOutputPath.Name = "ButtonCanvasSelectOutputPath";
            this.ButtonCanvasSelectOutputPath.Size = new System.Drawing.Size(29, 20);
            this.ButtonCanvasSelectOutputPath.TabIndex = 2;
            this.ButtonCanvasSelectOutputPath.Text = "...";
            this.ButtonCanvasSelectOutputPath.UseVisualStyleBackColor = true;
            this.ButtonCanvasSelectOutputPath.Click += new System.EventHandler(this.ButtonCanvasSelectOutputPath_Click);
            // 
            // LabelCanvasStatus
            // 
            this.LabelCanvasStatus.AutoSize = true;
            this.LabelCanvasStatus.Location = new System.Drawing.Point(8, 97);
            this.LabelCanvasStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LabelCanvasStatus.Name = "LabelCanvasStatus";
            this.LabelCanvasStatus.Size = new System.Drawing.Size(57, 15);
            this.LabelCanvasStatus.TabIndex = 8;
            this.LabelCanvasStatus.Text = "Waiting...";
            // 
            // ButtonCanvasCancel
            // 
            this.ButtonCanvasCancel.Enabled = false;
            this.ButtonCanvasCancel.Location = new System.Drawing.Point(255, 340);
            this.ButtonCanvasCancel.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCanvasCancel.Name = "ButtonCanvasCancel";
            this.ButtonCanvasCancel.Size = new System.Drawing.Size(78, 20);
            this.ButtonCanvasCancel.TabIndex = 9;
            this.ButtonCanvasCancel.Text = "&Cancel";
            this.ButtonCanvasCancel.UseVisualStyleBackColor = true;
            this.ButtonCanvasCancel.Click += new System.EventHandler(this.ButtonCanvasCancel_Click);
            // 
            // SFDMain
            // 
            this.SFDMain.DefaultExt = "png";
            this.SFDMain.Filter = "PNG Images (*.png)|*.png|All files|*.*";
            this.SFDMain.Title = "PictureTiler";
            // 
            // OFDMain
            // 
            this.OFDMain.DefaultExt = "png";
            this.OFDMain.Filter = "All files|*.*";
            this.OFDMain.Title = "PictureTiler";
            // 
            // CheckTreatInputAsPathsFile
            // 
            this.CheckTreatInputAsPathsFile.AutoSize = true;
            this.CheckTreatInputAsPathsFile.Location = new System.Drawing.Point(8, 6);
            this.CheckTreatInputAsPathsFile.Name = "CheckTreatInputAsPathsFile";
            this.CheckTreatInputAsPathsFile.Size = new System.Drawing.Size(260, 19);
            this.CheckTreatInputAsPathsFile.TabIndex = 10;
            this.CheckTreatInputAsPathsFile.Text = "Input names a file that contains image paths";
            this.CheckTreatInputAsPathsFile.UseVisualStyleBackColor = true;
            this.CheckTreatInputAsPathsFile.CheckedChanged += new System.EventHandler(this.CheckTreatInputAsPathsFile_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 407);
            this.Controls.Add(this.TabsMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "PictureTiler";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.TabsMain.ResumeLayout(false);
            this.TabTiler.ResumeLayout(false);
            this.TabTiler.PerformLayout();
            this.GroupTilerOptions.ResumeLayout(false);
            this.GroupTilerOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDTilerTileHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDTilerTileWidth)).EndInit();
            this.TabPacker.ResumeLayout(false);
            this.TabPacker.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.TabCanvas.ResumeLayout(false);
            this.TabCanvas.PerformLayout();
            this.GroupCanvas.ResumeLayout(false);
            this.GroupCanvas.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl TabsMain;
		private System.Windows.Forms.TabPage TabTiler;
		private System.Windows.Forms.Button ButtonTilerSelectOutputPath;
		private System.Windows.Forms.TextBox TextTilerOutputPath;
		private System.Windows.Forms.Label StaticLabelTilerOutputPath;
		private System.Windows.Forms.Button ButtonTilerSelectInputFolder;
		private System.Windows.Forms.TextBox TextTilerInputFolder;
		private System.Windows.Forms.Label StaticLabelTilerInputPath;
		private System.Windows.Forms.TabPage TabPacker;
		private System.Windows.Forms.Label LabelTilerStatus;
		private System.Windows.Forms.ProgressBar ProgressTiler;
		private System.Windows.Forms.GroupBox GroupTilerOptions;
		private System.Windows.Forms.Button ButtonTilerGenerate;
		private System.Windows.Forms.NumericUpDown NUDTilerTileHeight;
		private System.Windows.Forms.Label StaticLabelTilerTileHeight;
		private System.Windows.Forms.NumericUpDown NUDTilerTileWidth;
		private System.Windows.Forms.Label StaticLabelTilerTileWidth;
		private System.Windows.Forms.FolderBrowserDialog FBDMain;
		private System.Windows.Forms.SaveFileDialog SFDMain;
		private System.Windows.Forms.Button ButtonTilerCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label StaticLabelPackerOutputPath;
		private System.Windows.Forms.Button ButtonPackerGenerate;
		private System.Windows.Forms.Label StaticLabelPackerInputPath;
		private System.Windows.Forms.TextBox TextPackerInputPath;
		private System.Windows.Forms.Button ButtonPackerSelectInputPath;
		private System.Windows.Forms.TextBox TextPackerOutputPath;
		private System.Windows.Forms.Button ButtonPackerSelectOutputPath;
		private System.Windows.Forms.ProgressBar ProgressPacker;
		private System.Windows.Forms.Button ButtonPackerCancel;
		private System.Windows.Forms.Label LabelPackerStatus;
		private System.Windows.Forms.TabPage TabCanvas;
		private System.Windows.Forms.CheckBox CheckPackerRecursive;
		private System.Windows.Forms.CheckBox CheckPackerMultipicture;
		private System.Windows.Forms.GroupBox GroupCanvas;
		private System.Windows.Forms.Label StaticLabelCanvasOutputPath;
		private System.Windows.Forms.Button ButtonCanvasGenerate;
		private System.Windows.Forms.Label StaticLabelCanvasInputPath;
		private System.Windows.Forms.TextBox TextCanvasInputPath;
		private System.Windows.Forms.Button ButtonCanvasSelectInputPath;
		private System.Windows.Forms.TextBox TextCanvasOutputPath;
		private System.Windows.Forms.Button ButtonCanvasSelectOutputPath;
		private System.Windows.Forms.Label LabelCanvasStatus;
		private System.Windows.Forms.Button ButtonCanvasCancel;
		private System.Windows.Forms.OpenFileDialog OFDMain;
        private System.Windows.Forms.CheckBox CheckTreatInputAsPathsFile;
    }
}

