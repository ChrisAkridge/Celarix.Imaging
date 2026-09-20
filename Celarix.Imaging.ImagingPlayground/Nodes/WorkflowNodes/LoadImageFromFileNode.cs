using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Celarix.Imaging;
using System;
using System.Collections.Generic;
using System.Text;
using Celarix.Imaging.Utilities;

namespace Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes
{
    internal sealed class LoadImageFromFileNode : IWorkflowNode
    {
        public bool Ready => true;
        public bool Completed { get; private set; }

        // Settings
        public string FilePath { get; }

        // Outputs
        [OutputProperty]
        public ValueConnector<Image<Rgba32>>? Image { get; private set; }

        public LoadImageFromFileNode(string filePath)
        {
            FilePath = filePath;
        }

        public void SetInputConnectors(IEnumerable<ValueConnector> inputs)
        {
            if (inputs.Any())
            {
                throw new ArgumentException("LoadImageFromFileNode does not accept any input connectors.");
            }
        }

        public void SetOutputConnectors(IEnumerable<ValueConnector> outputs)
        {
            Image = outputs.Required<Image<Rgba32>>("Image");
        }

        public IEnumerable<IWorkflowNode> GetInputs()
        {
            return [];
        }

        public IEnumerable<IWorkflowNode> GetOutputs()
        {
            if (Image == null) { return []; }

            return Image.Tos;
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            if (Image == null)
            {
                throw new InvalidOperationException("Output connector 'Image' is not set.");
            }

            var result = await ImageLoader.LoadImage(FilePath, cancellationToken);
            if (result == null) { throw new InvalidOperationException($"Failed to load image at {FilePath}."); }
            else if (result.Result != ImageLoadAttemptResult.Success)
            {
                throw new InvalidOperationException($"Failed to load image at {FilePath}. Result: {result.Result}, Exception: {result.Exception}", result.Exception);
            }
            else if (result.LoadedImage == null)
            {
                throw new InvalidOperationException($"Loaded image is null for file path {FilePath}.");
            }

            Image.SetResult(result.LoadedImage);
            Completed = true;
        }
    }
}
