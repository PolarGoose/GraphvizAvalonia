namespace GraphvizAvalonia;

public class GraphDescription
{
    public class Node(object viewModel)
    {
        internal string Name { get; } = "node_" + Guid.NewGuid().ToString("N");
        public object ViewModel { get; } = viewModel;
    }

    public class Edge(Node from, Node to)
    {
        public enum ArrowShape
        {
            Box,
            Crow,
            Curve,
            Icurve,
            Diamond,
            Dot,
            Inv,
            None,
            Normal,
            Tee,
            Vee
        }

        public enum LineStyleType
        {
            Solid,
            Dashed,
            Dotted
        }

        internal string Id { get; } = "edge_" + Guid.NewGuid().ToString("N");
        public Node From { get; } = from;
        public Node To { get; } = to;
        public ArrowShape ArrowHead { get; set; } = ArrowShape.Normal;
        public ArrowShape ArrowTail { get; set; } = ArrowShape.None;
        public LineStyleType LineStyle { get; set; } = LineStyleType.Solid;
        public double LineWidth { get; set; } = 1.0;
        public string Label { get; set; } = string.Empty;
        public string HeadLabel { get; set; } = string.Empty;
        public string TailLabel { get; set; } = string.Empty;
        public Avalonia.Media.Color Color { get; set; } = Avalonia.Media.Colors.Black;
        public int FontSize { get; set; } = 12;
        public string FontName { get; set; } = "Consolas";
    }

    public class Cluster(object viewModel)
    {
        internal string Name { get; } = "cluster_" + Guid.NewGuid().ToString("N");
        public object ViewModel { get; } = viewModel;
        public List<Node> Nodes { get; } = [];
        public List<Cluster> Subgraphs { get; } = [];

        public void AddNode(Node node) => Nodes.Add(node);
        public void AddCluster(Cluster subgraph) => Subgraphs.Add(subgraph);
    }

    public List<Node> Vertices { get; } = [];
    public List<Edge> Edges { get; } = [];
    public List<Cluster> Subgraphs { get; } = [];

    public void AddNode(Node vertex) => Vertices.Add(vertex);
    public void AddSubgraph(Cluster subgraph) => Subgraphs.Add(subgraph);
    public void AddEdge(Edge edge) => Edges.Add(edge);
}
