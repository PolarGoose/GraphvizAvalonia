using Avalonia.Media;
using GraphvizAvalonia;

namespace Example.GraphsSamples;

internal class ComplexGraph : GraphSampleBase
{
    public override string Name => "Complex Graph";
    public override GraphDescription Description { get; }

    public ComplexGraph()
    {
        var start = new GraphDescription.Node(new NodeModel("Start"));
        var validate = new GraphDescription.Node(new NodeModel("Validate"));
        var process = new GraphDescription.Node(new NodeModel("Process"));
        var decideA = new GraphDescription.Node(new NodeModel("Decision A"));
        var decideB = new GraphDescription.Node(new NodeModel("Decision B"));
        var success = new GraphDescription.Node(new NodeModel("Success"));
        var failure = new GraphDescription.Node(new NodeModel("Failure"));
        var retry = new GraphDescription.Node(new NodeModel("Retry"));
        var logging = new GraphDescription.Node(new NodeModel("Logging"));

        var mainCluster = new GraphDescription.Cluster(
            new SubgraphModel(Brushes.LightGray, Brushes.DarkGray));
        mainCluster.AddNode(validate);
        mainCluster.AddNode(process);

        var decisionCluster = new GraphDescription.Cluster(
            new SubgraphModel(Brushes.LightBlue, Brushes.DarkBlue));
        decisionCluster.AddNode(decideA);
        decisionCluster.AddNode(decideB);

        var outcomeCluster = new GraphDescription.Cluster(
            new SubgraphModel(Brushes.LightGreen, Brushes.DarkGreen));
        outcomeCluster.AddNode(success);
        outcomeCluster.AddNode(failure);
        outcomeCluster.AddNode(retry);

        mainCluster.AddCluster(decisionCluster);
        mainCluster.AddCluster(outcomeCluster);

        var loggingCluster = new GraphDescription.Cluster(
            new SubgraphModel(Brushes.LightPink, Brushes.DarkRed));
        loggingCluster.AddNode(logging);

        Description = new GraphDescription();

        Description.AddNode(start);

        Description.AddSubgraph(mainCluster);
        Description.AddSubgraph(loggingCluster);

        var e1 = new GraphDescription.Edge(start, validate)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Box,
            ArrowTail = GraphDescription.Edge.ArrowShape.None,
            LineStyle = GraphDescription.Edge.LineStyleType.Solid,
            LineWidth = 2.0,
            Label = "Enter",
            HeadLabel = "to validate",
            TailLabel = "user",
            Color = Colors.DarkBlue,
            FontSize = 14
        };

        var e2 = new GraphDescription.Edge(validate, process)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Crow,
            ArrowTail = GraphDescription.Edge.ArrowShape.Box,
            LineStyle = GraphDescription.Edge.LineStyleType.Dashed,
            LineWidth = 1.5,
            Label = "Valid",
            HeadLabel = "ok",
            TailLabel = "input",
            Color = Colors.DarkGreen,
            FontSize = 12
        };

        var e3 = new GraphDescription.Edge(process, decideA)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Curve,
            ArrowTail = GraphDescription.Edge.ArrowShape.None,
            LineStyle = GraphDescription.Edge.LineStyleType.Dotted,
            LineWidth = 1.0,
            Label = "Route A",
            HeadLabel = "A?",
            TailLabel = string.Empty,
            Color = Colors.DarkRed,
            FontSize = 10
        };

        var e4 = new GraphDescription.Edge(process, decideB)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Icurve,
            ArrowTail = GraphDescription.Edge.ArrowShape.Curve,
            LineStyle = GraphDescription.Edge.LineStyleType.Solid,
            LineWidth = 2.5,
            Label = "Route B",
            HeadLabel = "B?",
            TailLabel = "alt",
            Color = Colors.OrangeRed,
            FontSize = 13
        };

        var e5 = new GraphDescription.Edge(decideA, success)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Diamond,
            ArrowTail = GraphDescription.Edge.ArrowShape.None,
            LineStyle = GraphDescription.Edge.LineStyleType.Dashed,
            LineWidth = 1.2,
            Label = "Yes",
            HeadLabel = "✔",
            TailLabel = "A",
            Color = Colors.ForestGreen,
            FontSize = 16
        };

        var e6 = new GraphDescription.Edge(decideA, failure)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Dot,
            ArrowTail = GraphDescription.Edge.ArrowShape.Diamond,
            LineStyle = GraphDescription.Edge.LineStyleType.Dotted,
            LineWidth = 1.0,
            Label = "No",
            HeadLabel = "✖",
            TailLabel = "A",
            Color = Colors.Crimson,
            FontSize = 11
        };

        var e7 = new GraphDescription.Edge(decideB, success)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Inv,
            ArrowTail = GraphDescription.Edge.ArrowShape.None,
            LineStyle = GraphDescription.Edge.LineStyleType.Solid,
            LineWidth = 1.8,
            Label = "OK",
            HeadLabel = "inv",
            TailLabel = "B",
            Color = Colors.DarkOliveGreen,
            FontSize = 15
        };

        var e8 = new GraphDescription.Edge(decideB, failure)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.None,
            ArrowTail = GraphDescription.Edge.ArrowShape.Inv,
            LineStyle = GraphDescription.Edge.LineStyleType.Dashed,
            LineWidth = 1.3,
            Label = "Fail",
            HeadLabel = "none",
            TailLabel = "B",
            Color = Colors.SaddleBrown,
            FontSize = 12
        };

        var e9 = new GraphDescription.Edge(failure, retry)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Normal,
            ArrowTail = GraphDescription.Edge.ArrowShape.None,
            LineStyle = GraphDescription.Edge.LineStyleType.Dotted,
            LineWidth = 1.1,
            Label = "Retry",
            HeadLabel = "try again",
            TailLabel = "error",
            Color = Colors.DarkMagenta,
            FontSize = 13
        };

        var e10 = new GraphDescription.Edge(retry, validate)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Tee,
            ArrowTail = GraphDescription.Edge.ArrowShape.Normal,
            LineStyle = GraphDescription.Edge.LineStyleType.Solid,
            LineWidth = 2.2,
            Label = "Re-validate",
            HeadLabel = "stop",
            TailLabel = "loop",
            Color = Colors.SteelBlue,
            FontSize = 14
        };

        var e11 = new GraphDescription.Edge(process, logging)
        {
            ArrowHead = GraphDescription.Edge.ArrowShape.Vee,
            ArrowTail = GraphDescription.Edge.ArrowShape.Tee,
            LineStyle = GraphDescription.Edge.LineStyleType.Dashed,
            LineWidth = 1.7,
            Label = "Log",
            HeadLabel = "log",
            TailLabel = "proc",
            Color = Colors.DarkSlateGray,
            FontSize = 12
        };

        Description.AddEdge(e1);
        Description.AddEdge(e2);
        Description.AddEdge(e3);
        Description.AddEdge(e4);
        Description.AddEdge(e5);
        Description.AddEdge(e6);
        Description.AddEdge(e7);
        Description.AddEdge(e8);
        Description.AddEdge(e9);
        Description.AddEdge(e10);
        Description.AddEdge(e11);
    }
}
