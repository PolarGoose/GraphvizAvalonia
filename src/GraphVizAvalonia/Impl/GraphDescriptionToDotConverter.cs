using System.Text;

namespace GraphvizAvalonia.Impl;

internal static class GraphDescriptionToDotConverter
{
    public static string ToDot(GraphDescription graphDescription, GraphCanvas graphCanvas)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph G { compound=true; node [shape=rect, fixedsize=true];");

        foreach (var vertex in graphDescription.Vertices)
        {
            AddControl(graphCanvas, sb, vertex, "  ");
        }

        foreach (var subgraph in graphDescription.Subgraphs)
        {
            AppendCluster(graphCanvas, sb, subgraph, "  ");
        }

        foreach (var edge in graphDescription.Edges)
        {
            sb.AppendLine($@"  {edge.From.Name} -> {edge.To.Name} [
    id={edge.Id}
    label=""{Escape(edge.Label)}""
    headlabel=""{Escape(edge.HeadLabel)}""
    taillabel=""{Escape(edge.TailLabel)}""
    arrowhead=""{Escape(edge.ArrowHead.ToString().ToLower())}""
    arrowtail=""{Escape(edge.ArrowTail.ToString().ToLower())}""
    fontsize={((Pts)(Dips)edge.FontSize).Value}
    fontname=""{edge.FontName}""
    dir=both
  ];
");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static void AppendCluster(Avalonia.Controls.Canvas graphCanvas, StringBuilder sb, GraphDescription.Cluster subgraph, string indent)
    {
        AddClusterControl(graphCanvas, sb, subgraph, indent);

        foreach (var vertex in subgraph.Nodes)
        {
            AddControl(graphCanvas, sb, vertex, $"{indent}  ");
        }

        foreach (var childSubgraph in subgraph.Subgraphs)
        {
            AppendCluster(graphCanvas, sb, childSubgraph, $"{indent}  ");
        }

        sb.AppendLine($"{indent}}}");
    }

    private static void AddControl(Avalonia.Controls.Canvas graphPanel, StringBuilder sb, GraphDescription.Node node, string indent)
    {
        var controlTemplate = Avalonia.Controls.Templates.DataTemplateExtensions.FindDataTemplate(graphPanel, node.ViewModel);
        if (controlTemplate == null)
        {
            throw new InvalidOperationException($"No DataTemplate found for ViewModel of type {node.ViewModel.GetType()}");
        }

        var control = controlTemplate.Build(node.ViewModel)!;
        control.DataContext = node.ViewModel;
        graphPanel.Children.Add(control);
        control.Tag = node;

        control.Measure(Avalonia.Size.Infinity);

        sb.AppendLine($"{indent}{node.Name} [label=\"\", width={(Inches)(Dips)control.DesiredSize.Width}, height={(Inches)(Dips)control.DesiredSize.Height}];");
    }

    private static void AddClusterControl(Avalonia.Controls.Canvas graphPanel, StringBuilder sb, GraphDescription.Cluster cluster, string indent)
    {
        var controlTemplate = Avalonia.Controls.Templates.DataTemplateExtensions.FindDataTemplate(graphPanel, cluster.ViewModel);
        if (controlTemplate == null)
        {
            throw new InvalidOperationException($"No DataTemplate found for ViewModel of type {cluster.ViewModel.GetType()}");
        }

        var control = controlTemplate.Build(cluster.ViewModel)!;
        control.DataContext = cluster.ViewModel;
        graphPanel.Children.Add(control);
        control.Tag = cluster;

        sb.AppendLine($"{indent}subgraph {cluster.Name} {{");
    }

    private static string Escape(string input)
    {
        return input.Replace("\"", "\\\"").Replace("\r\n", "\\n").Replace("\n", "\\n");
    }
}
