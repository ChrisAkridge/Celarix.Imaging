namespace Celarix.Imaging.ByteView
{
	partial class VideoProcessForm
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
			TableRoot = new System.Windows.Forms.TableLayoutPanel();
			LabelEffect = new System.Windows.Forms.Label();
			LabelEffectNote = new System.Windows.Forms.Label();
			GroupInput = new System.Windows.Forms.GroupBox();
			TableInput = new System.Windows.Forms.TableLayoutPanel();
			TextInput = new System.Windows.Forms.TextBox();
			ButtonBrowseInput = new System.Windows.Forms.Button();
			GroupOutput = new System.Windows.Forms.GroupBox();
			TableOutput = new System.Windows.Forms.TableLayoutPanel();
			ComboFormat = new System.Windows.Forms.ComboBox();
			LabelFormatNotes = new System.Windows.Forms.Label();
			TextOutput = new System.Windows.Forms.TextBox();
			ButtonBrowseOutput = new System.Windows.Forms.Button();
			GroupRange = new System.Windows.Forms.GroupBox();
			TableRange = new System.Windows.Forms.TableLayoutPanel();
			CheckClip = new System.Windows.Forms.CheckBox();
			LabelStart = new System.Windows.Forms.Label();
			TextStart = new System.Windows.Forms.TextBox();
			LabelDuration = new System.Windows.Forms.Label();
			TextDuration = new System.Windows.Forms.TextBox();
			LabelRangeHint = new System.Windows.Forms.Label();
			ProgressMain = new System.Windows.Forms.ProgressBar();
			LabelStatus = new System.Windows.Forms.Label();
			PanelButtons = new System.Windows.Forms.FlowLayoutPanel();
			ButtonStart = new System.Windows.Forms.Button();
			ButtonClose = new System.Windows.Forms.Button();
			OFDInput = new System.Windows.Forms.OpenFileDialog();
			SFDOutput = new System.Windows.Forms.SaveFileDialog();
			TableRoot.SuspendLayout();
			GroupInput.SuspendLayout();
			TableInput.SuspendLayout();
			GroupOutput.SuspendLayout();
			TableOutput.SuspendLayout();
			GroupRange.SuspendLayout();
			TableRange.SuspendLayout();
			PanelButtons.SuspendLayout();
			SuspendLayout();
			//
			// TableRoot
			//
			TableRoot.AutoScroll = true;
			TableRoot.ColumnCount = 1;
			TableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			TableRoot.Controls.Add(LabelEffect, 0, 0);
			TableRoot.Controls.Add(LabelEffectNote, 0, 1);
			TableRoot.Controls.Add(GroupInput, 0, 2);
			TableRoot.Controls.Add(GroupOutput, 0, 3);
			TableRoot.Controls.Add(GroupRange, 0, 4);
			TableRoot.Controls.Add(ProgressMain, 0, 5);
			TableRoot.Controls.Add(LabelStatus, 0, 6);
			TableRoot.Controls.Add(PanelButtons, 0, 7);
			TableRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			TableRoot.Name = "TableRoot";
			TableRoot.Padding = new System.Windows.Forms.Padding(12);
			TableRoot.RowCount = 9;
			for (int i = 0; i < 8; i++)
			{
				TableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			}
			TableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			TableRoot.TabIndex = 0;
			//
			// LabelEffect
			//
			LabelEffect.AutoSize = true;
			LabelEffect.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			LabelEffect.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			LabelEffect.MaximumSize = new System.Drawing.Size(540, 0);
			LabelEffect.Name = "LabelEffect";
			//
			// LabelEffectNote
			//
			LabelEffectNote.AutoSize = true;
			LabelEffectNote.ForeColor = System.Drawing.SystemColors.GrayText;
			LabelEffectNote.Margin = new System.Windows.Forms.Padding(3, 0, 3, 9);
			LabelEffectNote.MaximumSize = new System.Drawing.Size(540, 0);
			LabelEffectNote.Name = "LabelEffectNote";
			//
			// GroupInput
			//
			GroupInput.AutoSize = true;
			GroupInput.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			GroupInput.Controls.Add(TableInput);
			GroupInput.Dock = System.Windows.Forms.DockStyle.Fill;
			GroupInput.Name = "GroupInput";
			GroupInput.Padding = new System.Windows.Forms.Padding(8);
			GroupInput.TabStop = false;
			GroupInput.Text = "Input video";
			//
			// TableInput
			//
			TableInput.AutoSize = true;
			TableInput.ColumnCount = 2;
			TableInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			TableInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			TableInput.Controls.Add(TextInput, 0, 0);
			TableInput.Controls.Add(ButtonBrowseInput, 1, 0);
			TableInput.Dock = System.Windows.Forms.DockStyle.Top;
			TableInput.Name = "TableInput";
			TableInput.RowCount = 1;
			TableInput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			//
			// TextInput
			//
			TextInput.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			TextInput.Name = "TextInput";
			TextInput.TabIndex = 0;
			TextInput.TextChanged += TextInput_TextChanged;
			//
			// ButtonBrowseInput
			//
			ButtonBrowseInput.AutoSize = true;
			ButtonBrowseInput.Name = "ButtonBrowseInput";
			ButtonBrowseInput.TabIndex = 1;
			ButtonBrowseInput.Text = "Browse...";
			ButtonBrowseInput.UseVisualStyleBackColor = true;
			ButtonBrowseInput.Click += ButtonBrowseInput_Click;
			//
			// GroupOutput
			//
			GroupOutput.AutoSize = true;
			GroupOutput.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			GroupOutput.Controls.Add(TableOutput);
			GroupOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			GroupOutput.Name = "GroupOutput";
			GroupOutput.Padding = new System.Windows.Forms.Padding(8);
			GroupOutput.TabStop = false;
			GroupOutput.Text = "Output";
			//
			// TableOutput
			//
			TableOutput.AutoSize = true;
			TableOutput.ColumnCount = 2;
			TableOutput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			TableOutput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			TableOutput.Controls.Add(ComboFormat, 0, 0);
			TableOutput.SetColumnSpan(ComboFormat, 2);
			TableOutput.Controls.Add(LabelFormatNotes, 0, 1);
			TableOutput.SetColumnSpan(LabelFormatNotes, 2);
			TableOutput.Controls.Add(TextOutput, 0, 2);
			TableOutput.Controls.Add(ButtonBrowseOutput, 1, 2);
			TableOutput.Dock = System.Windows.Forms.DockStyle.Top;
			TableOutput.Name = "TableOutput";
			TableOutput.RowCount = 3;
			TableOutput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			TableOutput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			TableOutput.RowStyles.Add(new System.Windows.Forms.RowStyle());
			//
			// ComboFormat
			//
			ComboFormat.Dock = System.Windows.Forms.DockStyle.Fill;
			ComboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			ComboFormat.Name = "ComboFormat";
			ComboFormat.TabIndex = 2;
			ComboFormat.SelectedIndexChanged += ComboFormat_SelectedIndexChanged;
			//
			// LabelFormatNotes
			//
			LabelFormatNotes.AutoSize = true;
			LabelFormatNotes.ForeColor = System.Drawing.SystemColors.GrayText;
			LabelFormatNotes.Margin = new System.Windows.Forms.Padding(3, 6, 3, 9);
			LabelFormatNotes.MaximumSize = new System.Drawing.Size(500, 0);
			LabelFormatNotes.Name = "LabelFormatNotes";
			//
			// TextOutput
			//
			TextOutput.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			TextOutput.Name = "TextOutput";
			TextOutput.TabIndex = 3;
			//
			// ButtonBrowseOutput
			//
			ButtonBrowseOutput.AutoSize = true;
			ButtonBrowseOutput.Name = "ButtonBrowseOutput";
			ButtonBrowseOutput.TabIndex = 4;
			ButtonBrowseOutput.Text = "Browse...";
			ButtonBrowseOutput.UseVisualStyleBackColor = true;
			ButtonBrowseOutput.Click += ButtonBrowseOutput_Click;
			//
			// GroupRange
			//
			GroupRange.AutoSize = true;
			GroupRange.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			GroupRange.Controls.Add(TableRange);
			GroupRange.Dock = System.Windows.Forms.DockStyle.Fill;
			GroupRange.Name = "GroupRange";
			GroupRange.Padding = new System.Windows.Forms.Padding(8);
			GroupRange.TabStop = false;
			GroupRange.Text = "Range";
			//
			// TableRange
			//
			TableRange.AutoSize = true;
			TableRange.ColumnCount = 4;
			TableRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			TableRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			TableRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			TableRange.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			TableRange.Controls.Add(CheckClip, 0, 0);
			TableRange.SetColumnSpan(CheckClip, 4);
			TableRange.Controls.Add(LabelStart, 0, 1);
			TableRange.Controls.Add(TextStart, 1, 1);
			TableRange.Controls.Add(LabelDuration, 2, 1);
			TableRange.Controls.Add(TextDuration, 3, 1);
			TableRange.Controls.Add(LabelRangeHint, 0, 2);
			TableRange.SetColumnSpan(LabelRangeHint, 4);
			TableRange.Dock = System.Windows.Forms.DockStyle.Top;
			TableRange.Name = "TableRange";
			TableRange.RowCount = 3;
			TableRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
			TableRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
			TableRange.RowStyles.Add(new System.Windows.Forms.RowStyle());
			//
			// CheckClip
			//
			CheckClip.AutoSize = true;
			CheckClip.Name = "CheckClip";
			CheckClip.TabIndex = 5;
			CheckClip.Text = "Only process part of the video";
			CheckClip.UseVisualStyleBackColor = true;
			CheckClip.CheckedChanged += CheckClip_CheckedChanged;
			//
			// LabelStart
			//
			LabelStart.Anchor = System.Windows.Forms.AnchorStyles.Left;
			LabelStart.AutoSize = true;
			LabelStart.Name = "LabelStart";
			LabelStart.Text = "Start at:";
			//
			// TextStart
			//
			TextStart.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			TextStart.Name = "TextStart";
			TextStart.TabIndex = 6;
			TextStart.Text = "0:00";
			//
			// LabelDuration
			//
			LabelDuration.Anchor = System.Windows.Forms.AnchorStyles.Left;
			LabelDuration.AutoSize = true;
			LabelDuration.Margin = new System.Windows.Forms.Padding(12, 0, 3, 0);
			LabelDuration.Name = "LabelDuration";
			LabelDuration.Text = "Length:";
			//
			// TextDuration
			//
			TextDuration.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			TextDuration.Name = "TextDuration";
			TextDuration.TabIndex = 7;
			TextDuration.Text = "0:01";
			//
			// LabelRangeHint
			//
			LabelRangeHint.AutoSize = true;
			LabelRangeHint.ForeColor = System.Drawing.SystemColors.GrayText;
			LabelRangeHint.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			LabelRangeHint.Name = "LabelRangeHint";
			LabelRangeHint.Text = "Times can be seconds (12.5), minutes and seconds (1:30), or hours too (0:01:30.25).";
			//
			// ProgressMain
			//
			ProgressMain.Dock = System.Windows.Forms.DockStyle.Fill;
			ProgressMain.Margin = new System.Windows.Forms.Padding(3, 12, 3, 3);
			ProgressMain.Maximum = 1000;
			ProgressMain.Name = "ProgressMain";
			ProgressMain.Size = new System.Drawing.Size(100, 23);
			ProgressMain.TabIndex = 8;
			//
			// LabelStatus
			//
			LabelStatus.AutoSize = true;
			LabelStatus.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
			LabelStatus.MaximumSize = new System.Drawing.Size(540, 0);
			LabelStatus.Name = "LabelStatus";
			LabelStatus.Text = "Ready.";
			//
			// PanelButtons
			//
			PanelButtons.AutoSize = true;
			PanelButtons.Controls.Add(ButtonClose);
			PanelButtons.Controls.Add(ButtonStart);
			PanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			PanelButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			PanelButtons.Name = "PanelButtons";
			//
			// ButtonStart
			//
			ButtonStart.AutoSize = true;
			ButtonStart.Name = "ButtonStart";
			ButtonStart.TabIndex = 9;
			ButtonStart.Text = "Process Video";
			ButtonStart.UseVisualStyleBackColor = true;
			ButtonStart.Click += ButtonStart_Click;
			//
			// ButtonClose
			//
			ButtonClose.AutoSize = true;
			ButtonClose.Name = "ButtonClose";
			ButtonClose.TabIndex = 10;
			ButtonClose.Text = "Close";
			ButtonClose.UseVisualStyleBackColor = true;
			ButtonClose.Click += ButtonClose_Click;
			//
			// OFDInput
			//
			OFDInput.Filter = "Video files|*.mp4;*.m4v;*.mkv;*.mov;*.avi;*.webm;*.wmv;*.flv;*.mpg;*.mpeg;*.ts;*.m2ts;*.3gp;*.ogv|All files|*.*";
			OFDInput.Title = "ByteView - Choose Input Video";
			//
			// SFDOutput
			//
			SFDOutput.Title = "ByteView - Choose Output Video";
			//
			// VideoProcessForm
			//
			AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(600, 620);
			Controls.Add(TableRoot);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "VideoProcessForm";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Process Video";
			FormClosing += VideoProcessForm_FormClosing;
			Load += VideoProcessForm_Load;
			TableRoot.ResumeLayout(false);
			TableRoot.PerformLayout();
			GroupInput.ResumeLayout(false);
			GroupInput.PerformLayout();
			TableInput.ResumeLayout(false);
			TableInput.PerformLayout();
			GroupOutput.ResumeLayout(false);
			GroupOutput.PerformLayout();
			TableOutput.ResumeLayout(false);
			TableOutput.PerformLayout();
			GroupRange.ResumeLayout(false);
			GroupRange.PerformLayout();
			TableRange.ResumeLayout(false);
			TableRange.PerformLayout();
			PanelButtons.ResumeLayout(false);
			PanelButtons.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel TableRoot;
		private System.Windows.Forms.Label LabelEffect;
		private System.Windows.Forms.Label LabelEffectNote;
		private System.Windows.Forms.GroupBox GroupInput;
		private System.Windows.Forms.TableLayoutPanel TableInput;
		private System.Windows.Forms.TextBox TextInput;
		private System.Windows.Forms.Button ButtonBrowseInput;
		private System.Windows.Forms.GroupBox GroupOutput;
		private System.Windows.Forms.TableLayoutPanel TableOutput;
		private System.Windows.Forms.ComboBox ComboFormat;
		private System.Windows.Forms.Label LabelFormatNotes;
		private System.Windows.Forms.TextBox TextOutput;
		private System.Windows.Forms.Button ButtonBrowseOutput;
		private System.Windows.Forms.GroupBox GroupRange;
		private System.Windows.Forms.TableLayoutPanel TableRange;
		private System.Windows.Forms.CheckBox CheckClip;
		private System.Windows.Forms.Label LabelStart;
		private System.Windows.Forms.TextBox TextStart;
		private System.Windows.Forms.Label LabelDuration;
		private System.Windows.Forms.TextBox TextDuration;
		private System.Windows.Forms.Label LabelRangeHint;
		private System.Windows.Forms.ProgressBar ProgressMain;
		private System.Windows.Forms.Label LabelStatus;
		private System.Windows.Forms.FlowLayoutPanel PanelButtons;
		private System.Windows.Forms.Button ButtonStart;
		private System.Windows.Forms.Button ButtonClose;
		private System.Windows.Forms.OpenFileDialog OFDInput;
		private System.Windows.Forms.SaveFileDialog SFDOutput;
	}
}
