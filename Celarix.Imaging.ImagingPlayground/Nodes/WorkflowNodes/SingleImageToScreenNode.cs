using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes
{
    internal sealed class SingleImageToScreenNode : IWorkflowNode
    {
        public bool Ready => Image?.Result != null;

        public bool Completed { get; private set; }

        // Inputs
        [InputProperty]
        public ValueConnector<Image<Rgba32>>? Image { get; private set; }

        public Task Run(CancellationToken cancellationToken)
        {
            if (Image?.Result != null)
            {
                Instance.ViewerForm?.DisplaySingleImage(Image.Result);
            }

            Completed = true;
            return Task.CompletedTask;
        }

        public void SetInputConnectors(IEnumerable<ValueConnector> inputs)
        {
            Image = inputs.Required<Image<Rgba32>>("Image");
        }

        public void SetOutputConnectors(IEnumerable<ValueConnector> outputs)
        {
            if (outputs.Any())
            {
                throw new InvalidOperationException($"SingleImageToScreenNode does not accept any output connectors.");
            }
        }

        public IEnumerable<IWorkflowNode> GetInputs()
        {
            if (Image?.From != null)
            {
                var input = Image.From;
                if (input != null)
                {
                    return [input];
                }
            }
            return [];
        }

        public IEnumerable<IWorkflowNode> GetOutputs()
        {
            return [];
        }
    }
}
