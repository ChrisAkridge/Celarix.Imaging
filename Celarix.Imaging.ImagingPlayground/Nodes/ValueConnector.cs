using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes
{
    internal abstract class ValueConnector
    {
        private readonly List<IWorkflowNode> _tos = [];

        public string Name { get; }
        public bool ResultAvailable { get; set; }

        public IWorkflowNode? From { get; set; } = null;
        public IReadOnlyList<IWorkflowNode> Tos => _tos;

        protected ValueConnector(string name)
        {
            Name = name;
        }

        public void AddTo(IWorkflowNode to)
        {
            _tos.Add(to);
        }
    }

    internal sealed class ValueConnector<TValue> : ValueConnector
    {
        public TValue? Result { get; private set; }

        public ValueConnector(string name) : base(name)
        {
        }

        public void SetResult(TValue value)
        {
            Result = value;
            ResultAvailable = true;
        }
    }
}
