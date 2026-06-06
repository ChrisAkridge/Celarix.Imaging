using Celarix.Imaging.ImagingPlayground.Nodes.WorkflowNodes;
using ST.Library.UI.NodeEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Celarix.Imaging.ImagingPlayground.Nodes
{
    internal sealed class WorkflowRunner
    {
        private sealed class ConstructingConnection
        {
            public required string Name { get; init; }
            public required STNode From { get; init; }
            public required STNodeOption Connection { get; init; }
            public required STNode To { get; init; }
            public Type? ConnectionType => Connection?.DataType;
        }

        private IReadOnlyList<IWorkflowNode> _workflowNodes;

        public WorkflowRunner(IEnumerable<STNode> stNodes)
        {
            // Build the graph of connections between the STNodes. This will be used to connect the workflow nodes together.
            var nodeList = stNodes.ToList();
            var connections = GetConnectionsFromSTNodes(nodeList);
            var workflowNodeMap = BuildWorkflowNodeMap(nodeList);
            BuildValueConnectors(nodeList, connections, workflowNodeMap);

            // Validate the workflow graph. This includes checking for cycles and ensuring that all required inputs are connected.
            var workflowNodes = workflowNodeMap.Values.ToList();
            NoCyclesOrThrow(workflowNodes);
            AllRequiredInputsConnectedOrThrow(workflowNodes);
            NoMultipleUsesOfSingleUseNodesOrThrow(workflowNodes);

            // Create a topological-order sort of the workflow nodes based on the connections between
            // them. This will be used to determine the order in which to run the workflow nodes.
            _workflowNodes = BuildTopologicalSort(workflowNodes);
        }

        public async Task RunWorkflow(CancellationToken cancellationToken)
        {
            foreach (var node in _workflowNodes)
            {
                if (!node.Ready)
                {
                    throw new InvalidOperationException($"Workflow node {node.GetType().Name} is not ready to run.");
                }
                await node.Run(cancellationToken);
            }
        }

        private static IReadOnlyList<ConstructingConnection> GetConnectionsFromSTNodes(List<STNode> nodeList)
        {
            var connections = new List<ConstructingConnection>();
            foreach (var node in nodeList)
            {
                var outputOptions = node.GetOutputOptions();
                foreach (var outputOption in outputOptions)
                {
                    var connectedOptions = outputOption.GetConnectedOption();
                    foreach (var connectedOption in connectedOptions)
                    {
                        var nextNode = connectedOption.Owner;
                        connections.Add(new ConstructingConnection
                        {
                            Name = outputOption.Text,
                            From = node,
                            Connection = outputOption,
                            To = nextNode
                        });
                    }
                }
            }
            return connections;
        }

        private static IReadOnlyDictionary<STNode, IWorkflowNode> BuildWorkflowNodeMap(List<STNode> nodeList)
        {
            var workflowNodeMap = new Dictionary<STNode, IWorkflowNode>();
            foreach (var node in nodeList)
            {
                if (node is not IWorkflowNodeCreator creator)
                {
                    throw new InvalidOperationException($"STNode {node.GetType().Name} does not implement IWorkflowNodeCreator.");
                }

                var workflowNode = creator.CreateWorkflowNode();
                workflowNodeMap[node] = workflowNode;
            }
            return workflowNodeMap;
        }

        private static void BuildValueConnectors(IReadOnlyList<STNode> nodeList,
            IReadOnlyList<ConstructingConnection> connections,
            IReadOnlyDictionary<STNode, IWorkflowNode> workflowNodeMap)
        {
            var inputsByNode = workflowNodeMap.ToDictionary(kvp => kvp.Value, kvp => new List<ValueConnector>());
            var outputsByNode = workflowNodeMap.ToDictionary(kvp => kvp.Value, kvp => new List<ValueConnector>());

            foreach (var connection in connections)
            {
                var fromWorkflowNode = workflowNodeMap[connection.From];
                var toWorkflowNode = workflowNodeMap[connection.To];

                var workflowType = connection.ConnectionType
                    ?? throw new InvalidOperationException("Connection type is null.");

                // Use reflection to create an instance of the ValueConnector<> type where the type
                // argument is the workflow type.
                ValueConnector? connector = outputsByNode[fromWorkflowNode].FirstOrDefault(c => c.Name == connection.Name);
                if (connector == null)
                {
                    var valueConnectorType = typeof(ValueConnector<>).MakeGenericType(workflowType);
                    connector = (ValueConnector?)Activator.CreateInstance(valueConnectorType, connection.Name)
                        ?? throw new InvalidOperationException("Failed to create ValueConnector instance.");
                    connector.From = fromWorkflowNode;
                    outputsByNode[fromWorkflowNode].Add(connector);
                }

                connector.AddTo(toWorkflowNode);
                inputsByNode[toWorkflowNode].Add(connector);
            }

            foreach (var workflowNode in workflowNodeMap.Values)
            {
                workflowNode.SetInputConnectors(inputsByNode[workflowNode]);
                workflowNode.SetOutputConnectors(outputsByNode[workflowNode]);
            }
        }

        private static void NoCyclesOrThrow(IReadOnlyList<IWorkflowNode> workflowNodes)
        {
            // Perform a depth-first search to check for cycles in the workflow graph. If a cycle is detected, throw an exception.
            // To avoid throwing on diamond-shaped graphs, keep track of visited nodes in the current path and overall visited nodes separately.
            var visitedOverall = new HashSet<IWorkflowNode>();
            foreach (var workflowNode in workflowNodes)
            {
                if (!visitedOverall.Contains(workflowNode))
                {
                    var visitedInCurrentPath = new HashSet<IWorkflowNode>();
                    if (HasCycle(workflowNode, visitedInCurrentPath, visitedOverall))
                    {
                        throw new InvalidOperationException("Cycle detected in workflow graph.");
                    }
                }
            }
        }

        private static bool HasCycle(IWorkflowNode node, HashSet<IWorkflowNode> visitedInCurrentPath, HashSet<IWorkflowNode> visitedOverall)
        {
            if (visitedInCurrentPath.Contains(node))
            {
                return true;
            }
            if (visitedOverall.Contains(node))
            {
                return false;
            }
            visitedInCurrentPath.Add(node);
            visitedOverall.Add(node);
            foreach (var output in node.GetOutputs())
            {
                if (HasCycle(output, visitedInCurrentPath, visitedOverall))
                {
                    return true;
                }
            }
            visitedInCurrentPath.Remove(node);
            return false;
        }

        private static void AllRequiredInputsConnectedOrThrow(IReadOnlyList<IWorkflowNode> workflowNodes)
        {
            // For each workflow node, check that all required input connectors are connected. If any required input is not connected, throw an exception.
            foreach (var node in workflowNodes)
            {
                var inputProperties = node.GetType().GetProperties()
                    .Where(p => p.PropertyType.IsGenericType
                        && p.GetCustomAttribute<InputPropertyAttribute>() != null
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(ValueConnector<>));
                foreach (var property in inputProperties)
                {
                    var requiredInput = property.GetCustomAttributes(typeof(OptionalInputAttribute), false).Length == 0;
                    if (requiredInput)
                    {
                        var input = (ValueConnector?)property.GetValue(node);
                        if (input?.From == null)
                        {
                            throw new InvalidOperationException($"Workflow node {node.GetType().Name} has an unconnected required input connector '{property.Name}'.");
                        }
                    }
                }
            }
        }

        private static void NoMultipleUsesOfSingleUseNodesOrThrow(IReadOnlyList<IWorkflowNode> workflowNodes)
        {
            // For each workflow node, check that it is not on the workflow multiple times if it is
            // marked with the SingleUseOnlyNodeAttribute. If a node is found to be used multiple times, throw an exception.
            var singleUseNodes = workflowNodes
                .Where(n => n.GetType().GetCustomAttributes(typeof(SingleUseOnlyNodeAttribute), false).Length > 0)
                .GroupBy(n => n.GetType().FullName)
                .Where(g => g.Count() > 1);
            var multipleUseSingleUseNodeNames = string.Join(", ", singleUseNodes.Select(n => n.GetType().Name));
            if (multipleUseSingleUseNodeNames.Length > 0)
            {
                throw new InvalidOperationException($"Single-use nodes used multiple times: {multipleUseSingleUseNodeNames}");
            }
        }

        private static IReadOnlyList<IWorkflowNode> BuildTopologicalSort(IReadOnlyList<IWorkflowNode> nodes)
        {
            // Based on Kahn's algorithm (https://en.wikipedia.org/wiki/Topological_sorting#Kahn's_algorithm)
            // In-degree support courtesy of OpenAI's Codex
            var indegrees = nodes.ToDictionary(node => node, _ => 0);

            foreach (var node in nodes)
            {
                foreach (var output in node.GetOutputs())
                {
                    indegrees[output] += 1;
                }
            }

            var ready = new Queue<IWorkflowNode>(indegrees.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));
            var sorted = new List<IWorkflowNode>();

            while (ready.Count > 0)
            {
                var node = ready.Dequeue();
                sorted.Add(node);

                foreach (var output in node.GetOutputs())
                {
                    indegrees[output] -= 1;
                    if (indegrees[output] == 0)
                    {
                        ready.Enqueue(output);
                    }
                }
            }

            if (sorted.Count != nodes.Count)
            {
                throw new InvalidOperationException("The workflow contains a cycle.");
            }

            return sorted;
        }
    }
}
