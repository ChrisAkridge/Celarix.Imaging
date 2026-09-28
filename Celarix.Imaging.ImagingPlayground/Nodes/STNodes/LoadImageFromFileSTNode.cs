using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Text;
using Rgba32Image = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;

namespace Celarix.Imaging.ImagingPlayground.Nodes.STNodes
{
    [STNode("/Image/LoadImage", "Celarix", "", "", "Loads an image from a file and outputs the file path.")]
    internal sealed class LoadImageFromFileSTNode : STNode, IWorkflowNodeCreator<LoadImageFromFileNode>
    {
        private STNodeControl _fileNameDisplay = null!;
        private STNodeControl _pickImageButton = null!;
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

        public LoadImageFromFileNode Create()
        {
            return new LoadImageFromFileNode(_filePath);
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Load Image From File";
            AutoSize = false;
            Size = new Size(220, 105);
            LetGetOptions = true;

            _outputFilePath = new STNodeOption("Image", typeof(Rgba32Image), false);
            OutputOptions.Add(_outputFilePath);

            _fileNameDisplay = new STNodeControl
            {
                Text = "",
                Location = new Point(10, 10),
                Size = new Size(200, 23)
            };
            Controls.Add(_fileNameDisplay);

            _pickImageButton = new STNodeControl
            {
                Text = "Pick Image",
                Location = new Point(10, 43),
                Size = new Size(100, 23)
            };
            _pickImageButton.MouseClick += PickImageButton_Click;
            Controls.Add(_pickImageButton);
        }

        private void PickImageButton_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new()
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.webp|All Files|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FilePath = ofd.FileName;
            }
        }
    }
}
