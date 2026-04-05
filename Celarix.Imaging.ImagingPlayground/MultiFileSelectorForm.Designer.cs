namespace Celarix.Imaging.ImagingPlayground
{
    partial class MultiFileSelectorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiFileSelectorForm));
            StaticLabelFiles = new Label();
            ListFilePaths = new ListBox();
            ButtonAddFiles = new Button();
            ButtonAddFolderTopLevel = new Button();
            AddFilesFromFolderRecursive = new Button();
            LabelFileInfo = new Label();
            ButtonCancel = new Button();
            ButtonOK = new Button();
            OFDAddFiles = new OpenFileDialog();
            FBDAddFolder = new FolderBrowserDialog();
            ButtonRemoveFile = new Button();
            ButtonClearFiles = new Button();
            SuspendLayout();
            // 
            // StaticLabelFiles
            // 
            StaticLabelFiles.AutoSize = true;
            StaticLabelFiles.Location = new Point(12, 9);
            StaticLabelFiles.Name = "StaticLabelFiles";
            StaticLabelFiles.Size = new Size(33, 15);
            StaticLabelFiles.TabIndex = 0;
            StaticLabelFiles.Text = "Files:";
            // 
            // ListFilePaths
            // 
            ListFilePaths.FormattingEnabled = true;
            ListFilePaths.Location = new Point(12, 27);
            ListFilePaths.Name = "ListFilePaths";
            ListFilePaths.Size = new Size(214, 394);
            ListFilePaths.TabIndex = 1;
            ListFilePaths.SelectedIndexChanged += ListFilePaths_SelectedIndexChanged;
            // 
            // ButtonAddFiles
            // 
            ButtonAddFiles.Location = new Point(232, 27);
            ButtonAddFiles.Name = "ButtonAddFiles";
            ButtonAddFiles.Size = new Size(230, 23);
            ButtonAddFiles.TabIndex = 2;
            ButtonAddFiles.Text = "Add &Files...";
            ButtonAddFiles.UseVisualStyleBackColor = true;
            ButtonAddFiles.Click += ButtonAddFiles_Click;
            // 
            // ButtonAddFolderTopLevel
            // 
            ButtonAddFolderTopLevel.Location = new Point(232, 56);
            ButtonAddFolderTopLevel.Name = "ButtonAddFolderTopLevel";
            ButtonAddFolderTopLevel.Size = new Size(230, 23);
            ButtonAddFolderTopLevel.TabIndex = 3;
            ButtonAddFolderTopLevel.Text = "Add Files from F&older... (top level only)";
            ButtonAddFolderTopLevel.UseVisualStyleBackColor = true;
            ButtonAddFolderTopLevel.Click += ButtonAddFolderTopLevel_Click;
            // 
            // AddFilesFromFolderRecursive
            // 
            AddFilesFromFolderRecursive.Location = new Point(232, 85);
            AddFilesFromFolderRecursive.Name = "AddFilesFromFolderRecursive";
            AddFilesFromFolderRecursive.Size = new Size(230, 23);
            AddFilesFromFolderRecursive.TabIndex = 4;
            AddFilesFromFolderRecursive.Text = "Add Files from Folder... (&recursive)";
            AddFilesFromFolderRecursive.UseVisualStyleBackColor = true;
            AddFilesFromFolderRecursive.Click += AddFilesFromFolderRecursive_Click;
            // 
            // LabelFileInfo
            // 
            LabelFileInfo.AutoSize = true;
            LabelFileInfo.Location = new Point(232, 111);
            LabelFileInfo.Name = "LabelFileInfo";
            LabelFileInfo.Size = new Size(223, 150);
            LabelFileInfo.TabIndex = 5;
            LabelFileInfo.Text = resources.GetString("LabelFileInfo.Text");
            // 
            // ButtonCancel
            // 
            ButtonCancel.Location = new Point(520, 447);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(75, 23);
            ButtonCancel.TabIndex = 6;
            ButtonCancel.Text = "&Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // ButtonOK
            // 
            ButtonOK.Location = new Point(439, 447);
            ButtonOK.Name = "ButtonOK";
            ButtonOK.Size = new Size(75, 23);
            ButtonOK.TabIndex = 7;
            ButtonOK.Text = "O&K";
            ButtonOK.UseVisualStyleBackColor = true;
            ButtonOK.Click += ButtonOK_Click;
            // 
            // OFDAddFiles
            // 
            OFDAddFiles.Multiselect = true;
            OFDAddFiles.Title = "Add Files...";
            // 
            // ButtonRemoveFile
            // 
            ButtonRemoveFile.Enabled = false;
            ButtonRemoveFile.Location = new Point(12, 427);
            ButtonRemoveFile.Name = "ButtonRemoveFile";
            ButtonRemoveFile.Size = new Size(75, 23);
            ButtonRemoveFile.TabIndex = 8;
            ButtonRemoveFile.Text = "Re&move";
            ButtonRemoveFile.UseVisualStyleBackColor = true;
            ButtonRemoveFile.Click += ButtonRemoveFile_Click;
            // 
            // ButtonClearFiles
            // 
            ButtonClearFiles.Enabled = false;
            ButtonClearFiles.Location = new Point(93, 427);
            ButtonClearFiles.Name = "ButtonClearFiles";
            ButtonClearFiles.Size = new Size(75, 23);
            ButtonClearFiles.TabIndex = 9;
            ButtonClearFiles.Text = "Cl&ear";
            ButtonClearFiles.UseVisualStyleBackColor = true;
            ButtonClearFiles.Click += ButtonClearFiles_Click;
            // 
            // MultiFileSelectorForm
            // 
            AcceptButton = ButtonOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = ButtonCancel;
            ClientSize = new Size(604, 482);
            Controls.Add(ButtonClearFiles);
            Controls.Add(ButtonRemoveFile);
            Controls.Add(ButtonOK);
            Controls.Add(ButtonCancel);
            Controls.Add(LabelFileInfo);
            Controls.Add(AddFilesFromFolderRecursive);
            Controls.Add(ButtonAddFolderTopLevel);
            Controls.Add(ButtonAddFiles);
            Controls.Add(ListFilePaths);
            Controls.Add(StaticLabelFiles);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MultiFileSelectorForm";
            Text = "Select Files";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label StaticLabelFiles;
        private ListBox ListFilePaths;
        private Button ButtonAddFiles;
        private Button ButtonAddFolderTopLevel;
        private Button AddFilesFromFolderRecursive;
        private Label LabelFileInfo;
        private Button ButtonCancel;
        private Button ButtonOK;
        private OpenFileDialog OFDAddFiles;
        private FolderBrowserDialog FBDAddFolder;
        private Button ButtonRemoveFile;
        private Button ButtonClearFiles;
    }
}