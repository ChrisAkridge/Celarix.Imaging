using Celarix.Imaging.Collections;
using Celarix.Imaging.IO.v2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes
{
    internal sealed class FilesToNamedStreamsNode : IWorkflowNode
    {
        public bool Ready => FilePaths?.Result != null;

        public bool Completed { get; private set; }

        // Inputs
        [InputProperty]
        public ValueConnector<string[]>? FilePaths { get; private set; }

        // Outputs
        [OutputProperty]
        public ValueConnector<NamedStreamFactory[]>? NamedStreams { get; private set; }

        public IEnumerable<IWorkflowNode> GetInputs()
        {
            if (FilePaths?.From != null)
            {
                yield return FilePaths.From;
            }
        }

        public IEnumerable<IWorkflowNode> GetOutputs()
        {
            if (NamedStreams?.Tos != null)
            {
                return NamedStreams.Tos;
            }
            return [];
        }

        public Task Run(CancellationToken cancellationToken)
        {
            if (FilePaths?.Result != null)
            {
                NamedStreams?.SetResult([.. FilePaths.Result.Select(p => new NamedStreamFactory(p))]);
                Completed = true;
            }
            return Task.CompletedTask;
        }

        public void SetInputConnectors(IEnumerable<ValueConnector> inputs)
        {
            FilePaths = inputs.Required<string[]>("File Paths");
        }

        public void SetOutputConnectors(IEnumerable<ValueConnector> outputs)
        {
            NamedStreams = outputs.Required<NamedStreamFactory[]>("Named Streams");
        }
    }
}
