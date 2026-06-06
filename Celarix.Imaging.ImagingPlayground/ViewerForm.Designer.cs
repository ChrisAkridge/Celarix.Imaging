namespace Celarix.Imaging.ImagingPlayground
{
    partial class ViewerForm
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
            infiniteCanvasControl1 = new Celarix.Imaging.ImagingPlayground.Rendering.InfiniteCanvasControl();
            SuspendLayout();
            // 
            // infiniteCanvasControl1
            // 
            infiniteCanvasControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            infiniteCanvasControl1.BackColor = Color.Black;
            infiniteCanvasControl1.Location = new Point(2, 2);
            infiniteCanvasControl1.Margin = new Padding(4, 3, 4, 3);
            infiniteCanvasControl1.Name = "infiniteCanvasControl1";
            infiniteCanvasControl1.Size = new Size(794, 445);
            infiniteCanvasControl1.TabIndex = 0;
            infiniteCanvasControl1.VSync = true;
            // 
            // ViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(infiniteCanvasControl1);
            Name = "ViewerForm";
            Text = "ImagingPlayground - Result";
            ResumeLayout(false);
        }

        #endregion

        private Rendering.InfiniteCanvasControl infiniteCanvasControl1;
    }
}