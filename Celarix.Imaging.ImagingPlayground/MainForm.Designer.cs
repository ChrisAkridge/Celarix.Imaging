namespace Celarix.Imaging.ImagingPlayground
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
            SplitOperationsSecond = new SplitContainer();
            ButtonOpenImage = new Button();
            SplitMainImageProperties = new SplitContainer();
            InfiniteCanvas = new Celarix.Imaging.ImagingPlayground.Rendering.InfiniteCanvasControl();
            MainProperties = new PropertyGrid();
            panel1 = new Panel();
            ButtonCancel = new Button();
            ButtonRerun = new Button();
            TextLog = new TextBox();
            ProgressMain = new ProgressBar();
            OFDMain = new OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)SplitOperationsSecond).BeginInit();
            SplitOperationsSecond.Panel1.SuspendLayout();
            SplitOperationsSecond.Panel2.SuspendLayout();
            SplitOperationsSecond.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SplitMainImageProperties).BeginInit();
            SplitMainImageProperties.Panel1.SuspendLayout();
            SplitMainImageProperties.Panel2.SuspendLayout();
            SplitMainImageProperties.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // SplitOperationsSecond
            // 
            SplitOperationsSecond.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SplitOperationsSecond.FixedPanel = FixedPanel.Panel1;
            SplitOperationsSecond.IsSplitterFixed = true;
            SplitOperationsSecond.Location = new Point(0, 0);
            SplitOperationsSecond.Name = "SplitOperationsSecond";
            // 
            // SplitOperationsSecond.Panel1
            // 
            SplitOperationsSecond.Panel1.AutoScroll = true;
            SplitOperationsSecond.Panel1.Controls.Add(ButtonOpenImage);
            // 
            // SplitOperationsSecond.Panel2
            // 
            SplitOperationsSecond.Panel2.Controls.Add(SplitMainImageProperties);
            SplitOperationsSecond.Size = new Size(1174, 570);
            SplitOperationsSecond.SplitterDistance = 273;
            SplitOperationsSecond.TabIndex = 0;
            // 
            // ButtonOpenImage
            // 
            ButtonOpenImage.Location = new Point(12, 12);
            ButtonOpenImage.Name = "ButtonOpenImage";
            ButtonOpenImage.Size = new Size(250, 23);
            ButtonOpenImage.TabIndex = 0;
            ButtonOpenImage.Text = "Open Image...";
            ButtonOpenImage.UseVisualStyleBackColor = true;
            ButtonOpenImage.Click += ButtonOpenImage_Click;
            // 
            // SplitMainImageProperties
            // 
            SplitMainImageProperties.Dock = DockStyle.Fill;
            SplitMainImageProperties.Location = new Point(0, 0);
            SplitMainImageProperties.Name = "SplitMainImageProperties";
            // 
            // SplitMainImageProperties.Panel1
            // 
            SplitMainImageProperties.Panel1.Controls.Add(InfiniteCanvas);
            // 
            // SplitMainImageProperties.Panel2
            // 
            SplitMainImageProperties.Panel2.Controls.Add(MainProperties);
            SplitMainImageProperties.Size = new Size(897, 570);
            SplitMainImageProperties.SplitterDistance = 567;
            SplitMainImageProperties.TabIndex = 0;
            // 
            // InfiniteCanvas
            // 
            InfiniteCanvas.BackColor = Color.Black;
            InfiniteCanvas.Dock = DockStyle.Fill;
            InfiniteCanvas.Location = new Point(0, 0);
            InfiniteCanvas.Margin = new Padding(4, 3, 4, 3);
            InfiniteCanvas.Name = "InfiniteCanvas";
            InfiniteCanvas.Size = new Size(567, 570);
            InfiniteCanvas.TabIndex = 0;
            InfiniteCanvas.VSync = true;
            // 
            // MainProperties
            // 
            MainProperties.BackColor = SystemColors.Control;
            MainProperties.Dock = DockStyle.Fill;
            MainProperties.Location = new Point(0, 0);
            MainProperties.Name = "MainProperties";
            MainProperties.Size = new Size(326, 570);
            MainProperties.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(ButtonCancel);
            panel1.Controls.Add(ButtonRerun);
            panel1.Controls.Add(TextLog);
            panel1.Controls.Add(ProgressMain);
            panel1.Location = new Point(0, 570);
            panel1.Name = "panel1";
            panel1.Size = new Size(1174, 128);
            panel1.TabIndex = 1;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtonCancel.Location = new Point(1078, 64);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(93, 23);
            ButtonCancel.TabIndex = 3;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // ButtonRerun
            // 
            ButtonRerun.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtonRerun.Enabled = false;
            ButtonRerun.Location = new Point(1078, 35);
            ButtonRerun.Name = "ButtonRerun";
            ButtonRerun.Size = new Size(93, 23);
            ButtonRerun.TabIndex = 2;
            ButtonRerun.Text = "Rerun";
            ButtonRerun.UseVisualStyleBackColor = true;
            // 
            // TextLog
            // 
            TextLog.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TextLog.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TextLog.Location = new Point(3, 35);
            TextLog.Multiline = true;
            TextLog.Name = "TextLog";
            TextLog.ReadOnly = true;
            TextLog.Size = new Size(1069, 81);
            TextLog.TabIndex = 1;
            // 
            // ProgressMain
            // 
            ProgressMain.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            ProgressMain.Location = new Point(3, 6);
            ProgressMain.Name = "ProgressMain";
            ProgressMain.Size = new Size(1168, 23);
            ProgressMain.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1174, 698);
            Controls.Add(panel1);
            Controls.Add(SplitOperationsSecond);
            Name = "MainForm";
            Text = "Imaging Playground";
            Load += MainForm_Load;
            SplitOperationsSecond.Panel1.ResumeLayout(false);
            SplitOperationsSecond.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitOperationsSecond).EndInit();
            SplitOperationsSecond.ResumeLayout(false);
            SplitMainImageProperties.Panel1.ResumeLayout(false);
            SplitMainImageProperties.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitMainImageProperties).EndInit();
            SplitMainImageProperties.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer SplitOperationsSecond;
        private SplitContainer SplitMainImageProperties;
        private Panel panel1;
        private PropertyGrid MainProperties;
        private Button ButtonCancel;
        private Button ButtonRerun;
        private TextBox TextLog;
        private ProgressBar ProgressMain;
        private Button ButtonOpenImage;
        private OpenFileDialog OFDMain;
        private Rendering.InfiniteCanvasControl InfiniteCanvas;
    }
}
