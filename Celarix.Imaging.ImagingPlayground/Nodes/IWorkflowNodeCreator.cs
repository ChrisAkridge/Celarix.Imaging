using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes
{
    internal interface IWorkflowNodeCreator
    {
        IWorkflowNode CreateWorkflowNode();
    }

    internal interface IWorkflowNodeCreator<out TWorkflowNode> : IWorkflowNodeCreator
        where TWorkflowNode : IWorkflowNode
    {
        TWorkflowNode Create();
    }
}
