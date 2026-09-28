namespace Celarix.Imaging.ImagingPlayground
{
    partial class NodeForm
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
            NodeEditor = new ST.Library.UI.NodeEditor.STNodeEditor();
            ButtonRunFlow = new Button();
            ButtonCancel = new Button();
            PBFlowProgress = new ProgressBar();
            PBStepProgress = new ProgressBar();
            TextLog = new TextBox();
            NodeTreeView = new ST.Library.UI.NodeEditor.STNodeTreeView();
            NodePropertyGrid = new ST.Library.UI.NodeEditor.STNodePropertyGrid();
            SuspendLayout();
            // 
            // NodeEditor
            // 
            NodeEditor.AllowDrop = true;
            NodeEditor.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            NodeEditor.BackColor = Color.FromArgb(34, 34, 34);
            NodeEditor.Location = new Point(218, 12);
            NodeEditor.MarkForeColor = Color.FromArgb(180, 0, 0, 0);
            NodeEditor.MinimumSize = new Size(100, 100);
            NodeEditor.Name = "NodeEditor";
            NodeEditor.Size = new Size(570, 400);
            NodeEditor.TabIndex = 0;
            NodeEditor.Text = "stNodeEditor1";
            // 
            // ButtonRunFlow
            // 
            ButtonRunFlow.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonRunFlow.Location = new Point(713, 418);
            ButtonRunFlow.Name = "ButtonRunFlow";
            ButtonRunFlow.Size = new Size(75, 23);
            ButtonRunFlow.TabIndex = 2;
            ButtonRunFlow.Text = "&Run";
            ButtonRunFlow.UseVisualStyleBackColor = true;
            ButtonRunFlow.Click += this.ButtonRunFlow_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonCancel.Enabled = false;
            ButtonCancel.Location = new Point(713, 447);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(75, 23);
            ButtonCancel.TabIndex = 3;
            ButtonCancel.Text = "&Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // PBFlowProgress
            // 
            PBFlowProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PBFlowProgress.Location = new Point(12, 418);
            PBFlowProgress.Name = "PBFlowProgress";
            PBFlowProgress.Size = new Size(695, 23);
            PBFlowProgress.TabIndex = 4;
            // 
            // PBStepProgress
            // 
            PBStepProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PBStepProgress.Location = new Point(12, 447);
            PBStepProgress.Name = "PBStepProgress";
            PBStepProgress.Size = new Size(695, 23);
            PBStepProgress.TabIndex = 5;
            // 
            // TextLog
            // 
            TextLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextLog.Location = new Point(12, 476);
            TextLog.Multiline = true;
            TextLog.Name = "TextLog";
            TextLog.ReadOnly = true;
            TextLog.Size = new Size(776, 97);
            TextLog.TabIndex = 6;
            // 
            // NodeTreeView
            // 
            NodeTreeView.AllowDrop = true;
            NodeTreeView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            NodeTreeView.BackColor = Color.FromArgb(35, 35, 35);
            NodeTreeView.ForeColor = Color.FromArgb(220, 220, 220);
            NodeTreeView.Location = new Point(12, 12);
            NodeTreeView.MinimumSize = new Size(100, 60);
            NodeTreeView.Name = "NodeTreeView";
            NodeTreeView.ShowFolderCount = true;
            NodeTreeView.Size = new Size(200, 249);
            NodeTreeView.TabIndex = 7;
            NodeTreeView.Text = "stNodeTreeView1";
            // 
            // NodePropertyGrid
            // 
            NodePropertyGrid.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            NodePropertyGrid.BackColor = Color.FromArgb(35, 35, 35);
            NodePropertyGrid.ForeColor = Color.White;
            NodePropertyGrid.Location = new Point(12, 267);
            NodePropertyGrid.MinimumSize = new Size(120, 50);
            NodePropertyGrid.Name = "NodePropertyGrid";
            NodePropertyGrid.Size = new Size(200, 145);
            NodePropertyGrid.TabIndex = 8;
            NodePropertyGrid.Text = "stNodePropertyGrid1";
            // 
            // NodeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 585);
            Controls.Add(NodePropertyGrid);
            Controls.Add(NodeTreeView);
            Controls.Add(TextLog);
            Controls.Add(PBStepProgress);
            Controls.Add(PBFlowProgress);
            Controls.Add(ButtonCancel);
            Controls.Add(ButtonRunFlow);
            Controls.Add(NodeEditor);
            Name = "NodeForm";
            Text = "Imaging Playground";
            Load += NodeForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ST.Library.UI.NodeEditor.STNodeEditor NodeEditor;
        private Button ButtonRunFlow;
        private Button ButtonCancel;
        private ProgressBar PBFlowProgress;
        private ProgressBar PBStepProgress;
        private TextBox TextLog;
        private ST.Library.UI.NodeEditor.STNodeTreeView NodeTreeView;
        private ST.Library.UI.NodeEditor.STNodePropertyGrid NodePropertyGrid;
    }
}