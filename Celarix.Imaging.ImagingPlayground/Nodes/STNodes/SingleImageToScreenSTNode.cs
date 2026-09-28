using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Text;
using Rgba32Image = SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>;

namespace Celarix.Imaging.ImagingPlayground.Nodes.STNodes
{
    [STNode("/SingleImageToScreen", "Celarix", "", "", "Displays a single image on the viewer form.")]
    internal sealed class SingleImageToScreenSTNode : STNode, IWorkflowNodeCreator<SingleImageToScreenNode>
    {
        private STNodeOption _inputImage = null!;

        public IWorkflowNode CreateWorkflowNode() => Create();

        public SingleImageToScreenNode Create()
        {
            return new SingleImageToScreenNode();
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            Title = "Single Image To Screen";
            LetGetOptions = true;

            _inputImage = new STNodeOption("Image", typeof(Rgba32Image), true);
            InputOptions.Add(_inputImage);
        }
    }
}
