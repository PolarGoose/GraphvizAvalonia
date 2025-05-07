
using Avalonia.Media;
using GraphvizAvalonia;

namespace Example.GraphsSamples;

internal class SimpleGraph : GraphSampleBase
{
    public override string Name => "Simple Graph";
    public override GraphDescription Description { get; }

    public SimpleGraph()
    {
        var node1 = new GraphDescription.Node(new NodeModel("Node 1"));
        var node2 = new GraphDescription.Node(new NodeModel("Node 2"));
        var node3 = new GraphDescription.Node(new NodeModel("Node 3"));
        var node4 = new GraphDescription.Node(new NodeModel("Node 4"));
        var node5 = new GraphDescription.Node(new NodeModel("Node 5"));
        var node6 = new GraphDescription.Node(new NodeModel("Node 6"));

        var cluster = new GraphDescription.Cluster(new SubgraphModel(Brushes.LightGray, Brushes.DarkGray));
        cluster.AddNode(node2);
        cluster.AddNode(node3);

        var edge_node1_node2 = new GraphDescription.Edge(node1, node2);

        var edge_node1_node3 = new GraphDescription.Edge(node1, node3)
        {
            Label = "Label 🦆",
            ArrowHead = GraphDescription.Edge.ArrowShape.Diamond,
        };

        var edge_node2_node4 = new GraphDescription.Edge(node2, node4)
        {
            HeadLabel = "Head label",
            FontName = "Calibri",
            ArrowHead = GraphDescription.Edge.ArrowShape.Crow,
        };

        var edge_node4_node5 = new GraphDescription.Edge(node4, node5)
        {
            TailLabel = "Tail label",
            FontName = "Constantia",
            ArrowTail = GraphDescription.Edge.ArrowShape.Tee,
            ArrowHead = GraphDescription.Edge.ArrowShape.Box,
        };

        Description = new GraphDescription();
        Description.AddNode(node1);
        Description.AddNode(node4);
        Description.AddNode(node5);
        Description.AddEdge(edge_node1_node2);
        Description.AddEdge(edge_node1_node3);
        Description.AddEdge(edge_node2_node4);
        Description.AddEdge(edge_node4_node5);
        Description.AddSubgraph(cluster);
    }
}
