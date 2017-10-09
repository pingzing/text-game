using System.Collections.ObjectModel;

namespace TextGameExperiment.Core.Models.Graph
{
    public class NodeCollection<T> : Collection<GraphNode<T>>
    {
        public NodeCollection() : base() { }

        public NodeCollection(int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
            {
                base.Items.Add(default(GraphNode<T>));
            }            
        }

        public GraphNode<T> FindByValue(T value)
        {
            foreach (GraphNode<T> node in Items)
            {
                if (node.Value.Equals(value))
                {
                    return node;
                }
            }

            return null;
        }
    }
}
