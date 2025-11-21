using Avalonia.Controls;
using GraphvizAvalonia.Impl;

namespace GraphvizAvalonia;

public class GraphCanvas : Canvas
{
    public void DrawGraph(GraphDescription graphDescription, GraphvizLayoutEngine layoutEngine)
    {
        Children.Clear();

        // This also creates all necessary controls and adds them to `this.Children` collection.
        // It also sets their `Tag` property to the corresponding Node or Cluster description.
        var graphVizInputInDotLanguage = GraphDescriptionToDotConverter.ToDot(graphDescription, this);

        var graphVizJsonOutputAsString = GraphvizInterop.GenerateJsonOutput(graphVizInputInDotLanguage, layoutEngine);
        var graphVizJsonOutput = GraphvizJsonOutput.Deserialize(graphVizJsonOutputAsString);
        var deserializedGraph = new DeserializedGraphvizJsonOutput(graphVizJsonOutput);

        Width = deserializedGraph.BB.Width.Value;
        Height = deserializedGraph.BB.Height.Value;

        foreach (var control in Children)
        {
            switch (control.Tag)
            {
                case GraphDescription.Node nodeDescription:
                    var node = deserializedGraph.Nodes[nodeDescription.Name];
                    SetLeft(control, node.Position.X.Value);
                    SetTop(control, node.Position.Y.Value);
                    control.Width = node.Width.Value;
                    control.Height = node.Height.Value;
                    break;
                case GraphDescription.Cluster clusterDescription:
                    // Some layout engines don't support clusters
                    if(!deserializedGraph.Clusters.ContainsKey(clusterDescription.Name))
                        break;

                    var cluster = deserializedGraph.Clusters[clusterDescription.Name];
                    SetLeft(control, cluster.BoundingBox.Position.X.Value);
                    SetTop(control, cluster.BoundingBox.Position.Y.Value);
                    control.Width = cluster.BoundingBox.Width.Value;
                    control.Height = cluster.BoundingBox.Height.Value;
                    break;
            }
        }

        foreach (var edgeDescription in graphDescription.Edges)
        {
            var edge = deserializedGraph.Edges[edgeDescription.Id];

            // Only curves use specific `LineStyle`. The arrow heads and labels use `LineStyleType.Solid`
            new GraphvizDrawCommandsExecutor(this, edgeDescription, edge.Curve, edgeDescription.LineStyle).Execute();
            new GraphvizDrawCommandsExecutor(this, edgeDescription, edge.CurveLabel).Execute();
            new GraphvizDrawCommandsExecutor(this, edgeDescription, edge.TailArrowhead).Execute();
            new GraphvizDrawCommandsExecutor(this, edgeDescription, edge.TailArrowheadTextLabel).Execute();
            new GraphvizDrawCommandsExecutor(this, edgeDescription, edge.HeadArrowhead).Execute();
            new GraphvizDrawCommandsExecutor(this, edgeDescription, edge.HeadArrowheadTextLabel).Execute();
        }
    }
}
