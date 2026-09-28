using Celarix.Imaging.Collections;
using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using Celarix.Imaging.IO.v2;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes.STNodes
{
    [STNode("FilesToNamedStreams", "Celarix", "", "", "Converts a list of file paths to a list of named streams of bytes.")]
    internal class FilesToNamedStreamsSTNodes : STNode, IWorkflowNodeCreator<FilesToNamedStreamsNode>
    {
        private STNodeOption _inputFilePaths = null!;
        private STNodeOption _outputFilePaths = null!;

        public IWorkflowNode CreateWorkflowNode() => Create();

        public FilesToNamedStreamsNode Create()
        {
            return new FilesToNamedStreamsNode();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Files To Named Streams";
            AutoSize = true;
            LetGetOptions = true;
            _inputFilePaths = new STNodeOption("File Paths", typeof(string[]), true);
            InputOptions.Add(_inputFilePaths);
            _outputFilePaths = new STNodeOption("Named Streams", typeof(NamedStreamFactory[]), false);
            OutputOptions.Add(_outputFilePaths);
        }
    }
}
