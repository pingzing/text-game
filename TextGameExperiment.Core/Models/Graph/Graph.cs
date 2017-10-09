using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TextGameExperiment.Core.Models.Graph
{
    public class Graph<T> : IEnumerable<T>
    {
        public NodeCollection<T> Nodes { get; } = new NodeCollection<T>();
        public int Count => Nodes.Count;

        public Graph() { }
        public Graph(NodeCollection<T> nodeSet)
        {
            Nodes = nodeSet;
        }

        public void AddNode(GraphNode<T> node)
        {
            Nodes.Add(node);
        }

        public void AddNode(T value)
        {
            Nodes.Add(new GraphNode<T>(value));
        }

        public void AddDirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);
        }

        public void AddUndirectedEdge(GraphNode<T> from, GraphNode<T> to, int cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);

            to.Neighbors.Add(from);
            to.Costs.Add(cost);
        }

        public bool Contains(T value)
        {
            return Nodes.FindByValue(value) != null;
        }

        public bool Remove(T value)
        {
            // Find the node, or return false if it doesn't exist
            GraphNode<T> nodetoRemove = Nodes.FindByValue(value);
            if (nodetoRemove == null)
            {
                return false;
            }

            // Remove the node
            Nodes.Remove(nodetoRemove);

            // Remove all the node's edges            
            foreach (GraphNode<T> node in Nodes)
            {
                int index = node.Neighbors.IndexOf(nodetoRemove);
                if (index != -1)
                {
                    node.Neighbors.RemoveAt(index);
                    node.Costs.RemoveAt(index); // this _might_ be dangerous, if we ever change add logic
                }
            }

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Nodes.Select(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
