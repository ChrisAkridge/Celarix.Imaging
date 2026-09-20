using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes.STNodes
{
    [STNode("SingleFilePath", "Celarix", "", "", "Represents a single file path.")]
    internal sealed class SingleFilePathSTNode : STNode, IWorkflowNodeCreator<SingleFilePathNode>
    {
        private STNodeControl _fileNameDisplay = null!;
        private STNodeControl _pickFileButton = null!;
        private string _filePath = "";
        private STNodeOption _outputFilePath = null!;

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                _fileNameDisplay.Text = Path.GetFileName(_filePath);
                Invalidate();
            }
        }

        public IWorkflowNode CreateWorkflowNode() => Create();

        public SingleFilePathNode Create()
        {
            return new SingleFilePathNode(_filePath);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Single File Path";
            AutoSize = false;
            Size = new Size(220, 105);
            LetGetOptions = true;
            _outputFilePath = new STNodeOption("File Path", typeof(string[]), false);
            OutputOptions.Add(_outputFilePath);
            _fileNameDisplay = new STNodeControl
            {
                Text = "",
                Location = new Point(10, 10),
                Size = new Size(200, 23)
            };
            Controls.Add(_fileNameDisplay);
            _pickFileButton = new STNodeControl
            {
                Text = "Pick File",
                Location = new Point(10, 40),
                Size = new Size(200, 23)
            };
            _pickFileButton.MouseClick += (s, e) =>
            {
                using var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FilePath = ofd.FileName;
                    _outputFilePath.TransferData(FilePath);
                }
            };
            Controls.Add(_pickFileButton);
        }
    }
}
