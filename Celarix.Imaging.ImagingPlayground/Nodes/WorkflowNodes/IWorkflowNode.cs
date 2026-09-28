using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes
{
    internal interface IWorkflowNode
    {
        bool Ready { get; }
        bool Completed { get; }
        void SetInputConnectors(IEnumerable<ValueConnector> inputs);
        void SetOutputConnectors(IEnumerable<ValueConnector> outputs);
        IEnumerable<IWorkflowNode> GetInputs();
        IEnumerable<IWorkflowNode> GetOutputs();
        Task Run(CancellationToken cancellationToken);
    }
}
