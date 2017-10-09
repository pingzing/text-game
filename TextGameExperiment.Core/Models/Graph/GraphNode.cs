using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TextGameExperiment.Core.Models.Graph
{
    public class GraphNode<T>
    {
        public T Value { get; set; }
        public List<int> Costs { get; } = new List<int>();
        public NodeCollection<T> Neighbors { get; set; } = new NodeCollection<T>();

        public GraphNode() { }

        public GraphNode(T value)
        {
            Value = value;
        }

        public GraphNode(T value, NodeCollection<T> neighbors)
        {
            Value = value;
            Neighbors = neighbors;
        }        
    }
}
