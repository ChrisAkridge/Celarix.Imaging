using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes
{
    internal sealed class SingleFilePathNode : IWorkflowNode
    {
        public bool Ready => true;
        public bool Completed { get; private set; }

        // Settings
        public string FilePath { get; }

        // Outputs
        [OutputProperty]
        public ValueConnector<string[]>? FilePathOutput { get; private set; }

        public SingleFilePathNode(string filePath)
        {
            FilePath = filePath;
        }

        public void SetInputConnectors(IEnumerable<ValueConnector> inputs)
        {
            if (inputs.Any())
            {
                throw new ArgumentException("SingleFilePathNode does not accept any input connectors.");
            }
        }

        public void SetOutputConnectors(IEnumerable<ValueConnector> outputs)
        {
            FilePathOutput = outputs.Optional<string[]>("File Path");
        }

        public IEnumerable<IWorkflowNode> GetInputs()
        {
            return [];
        }

        public IEnumerable<IWorkflowNode> GetOutputs()
        {
            if (FilePathOutput == null) { return []; }
            return FilePathOutput.Tos;
        }

        public Task Run(CancellationToken cancellationToken)
        {
            if (FilePathOutput == null)
            {
                throw new InvalidOperationException("Output connector 'File Path' is not set.");
            }

            FilePathOutput.SetResult([FilePath]);
            Completed = true;
            return Task.CompletedTask;
        }
    }
}
