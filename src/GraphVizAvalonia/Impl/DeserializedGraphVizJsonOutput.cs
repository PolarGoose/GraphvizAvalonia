using System.Linq;

namespace GraphvizAvalonia.Impl;

// All coordinates and sizes are converted to DIPs (device-independent pixels).
// The coordinates are converted to the Avalonia's top-left based coordinate system.
internal sealed class DeserializedGraphvizJsonOutput
{
    internal sealed class Point
    {
        public Dips X { get; }
        public Dips Y { get; }

        public Point(Pts x, Pts y_graphviz, Pts yMax)
        {
            X = (Dips)x;
            Y = (Dips)(yMax - y_graphviz);
        }

        // pos is a string like "1.0,2.0"
        public Point(string pos, Pts yMax)
        {
            var parts = pos.Split(',').Select(s => (Pts)s).ToArray();
            var (x, y) = (parts[0], parts[1]);

            X = (Dips)x;
            Y = (Dips)(yMax - y);
        }

        public Point(Dips x, Dips y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Avalonia.Point(Point p)
        {
            return new Avalonia.Point(p.X.Value, p.Y.Value);
        }
    }

    internal sealed class BoundingBox
    {
        public Point Position { get; }
        public Dips Width { get; }
        public Dips Height { get; }

        // bb is a string like "x0,y0,x1,y1" in points
        public BoundingBox(string bb, Pts yMax)
        {
            var parts = bb.Split(',').Select(s => (Pts)s).ToArray();
            var (x0, y0, x1, y1) = (parts[0], parts[1], parts[2], parts[3]);
            Position = new Point(x0, y1, yMax);
            Width = (Dips)(x1 - x0);
            Height = (Dips)(y1 - y0);
        }
    }

    internal sealed class Node
    {
        public string Name { get; }
        public Point Position { get; }
        public Dips Height { get; }
        public Dips Width { get; }

        public Node(GraphvizJsonOutput.ClusterOrNode node, Pts yMax)
        {
            Name = node.name!;
            Height = (Dips)(Inches)node.height!;
            Width = (Dips)(Inches)node.width!;

            var centerPoint = new Point(node.pos!, yMax);
            Position = new Point(centerPoint.X - Width / 2, centerPoint.Y - Height / 2);
        }
    }

    internal sealed class Cluster(GraphvizJsonOutput.ClusterOrNode cluster, Pts yMax)
    {
        public string Name { get; } = cluster.name!;
        public BoundingBox BoundingBox { get; } = new BoundingBox(cluster.bb!, yMax);
    }

    internal class DrawCommand
    {
        internal static DrawCommand[] FromJson(GraphvizJsonOutput.DrawCommand[]? drawCommands, Pts yMax)
        {
            return drawCommands is null ? [] : [.. drawCommands.Select(cmd => FromJson(cmd!, yMax))];
        }

        private static DrawCommand FromJson(GraphvizJsonOutput.DrawCommand drawCommand, Pts yMax)
        {
            // Only handle the commands that are needed to draw the edges and their labels.
            switch (drawCommand.op)
            {
                case "e": return new EllipseDrawCommand(drawCommand, yMax);

                // "P" and "p" can be handled in the same way, because if the polygon is filled,
                // there will be SetFillColor command before it that will change the color from transparent to the required fill color.
                case "p":
                case "P":
                    return new PolygonDrawCommand(drawCommand, yMax);

                case "L": return new PolyLineDrawCommand(drawCommand, yMax);

                // "B" command is never used in the Json output.
                case "b": return new BSpline(drawCommand, yMax);

                case "T": return new TextDraw(drawCommand, yMax);
                case "C": return new SetFillColor(drawCommand);
                default:
                    return new DrawCommand();
            }
        }

        internal sealed class EllipseDrawCommand : DrawCommand
        {
            public Point Center { get; }
            public Dips W { get; } // The distance from the center to the furthest point on the ellipse along the x-axis.
            public Dips H { get; } // The distance from the center to the furthest point on the ellipse along the y-axis.

            internal EllipseDrawCommand(GraphvizJsonOutput.DrawCommand cmd, Pts yMax)
            {
                Center = new Point((Pts)cmd.rect![0], (Pts)cmd.rect[1], yMax);
                W = (Dips)(Pts)cmd.rect[2];
                H = (Dips)(Pts)cmd.rect[3];
            }
        }

        internal sealed class PolygonDrawCommand : DrawCommand
        {
            public Point[] Points { get; }
            internal PolygonDrawCommand(GraphvizJsonOutput.DrawCommand cmd, Pts yMax)
            {
                Points = [.. cmd.points.Select(p => new Point((Pts)p[0], (Pts)p[1], yMax))];
            }
        }

        internal sealed class PolyLineDrawCommand : DrawCommand
        {
            public Point[] Points { get; }
            internal PolyLineDrawCommand(GraphvizJsonOutput.DrawCommand cmd, Pts yMax)
            {
                Points = [.. cmd.points.Select(p => new Point((Pts)p[0], (Pts)p[1], yMax))];
            }
        }

        internal sealed class BSpline : DrawCommand
        {
            public Point StartPoint { get; }
            public IReadOnlyList<Point[]> Segments { get; }

            internal BSpline(GraphvizJsonOutput.DrawCommand cmd, Pts yMax)
            {
                // Graphviz's B-spline control points come as 3n+1 points.
                // The first point is the start, and then every group of 3 points
                // (starting at index 1) defines one cubic Bezier segment.

                var pts = cmd.points.Select(p => new Point((Pts)p[0], (Pts)p[1], yMax)).ToArray();

                StartPoint = pts[0];
                Segments = pts.Skip(1).Chunk(3).ToList();
            }
        }

        internal sealed class TextDraw : DrawCommand
        {
            public Point Center { get; }
            public Dips TextWidth { get; }
            public string Text { get; }

            internal TextDraw(GraphvizJsonOutput.DrawCommand cmd, Pts yMax)
            {
                Center = new Point((Pts)cmd.pt![0], (Pts)cmd.pt[1], yMax);
                TextWidth = (Dips)(Pts)cmd.width;
                Text = cmd.text!;
            }
        }

        internal sealed class SetFillColor(GraphvizJsonOutput.DrawCommand cmd) : DrawCommand
        {
            public string FillColor { get; } = cmd.color!;
        }
    }

    internal sealed class Edge
    {
        public string Id { get; }
        public DrawCommand[] Curve { get; }
        public DrawCommand[] CurveLabel { get; }
        public DrawCommand[] HeadArrowhead { get; }
        public DrawCommand[] HeadArrowheadTextLabel { get; }
        public DrawCommand[] TailArrowhead { get; }
        public DrawCommand[] TailArrowheadTextLabel { get; }

        internal Edge(GraphvizJsonOutput.Edge edge, Pts yMax)
        {
            Id = edge.id!;
            Curve = DrawCommand.FromJson(edge._draw_, yMax);
            CurveLabel = DrawCommand.FromJson(edge._ldraw_, yMax);
            HeadArrowhead = DrawCommand.FromJson(edge._hdraw_, yMax);
            HeadArrowheadTextLabel = DrawCommand.FromJson(edge._hldraw_, yMax);
            TailArrowhead = DrawCommand.FromJson(edge._tdraw_, yMax);
            TailArrowheadTextLabel = DrawCommand.FromJson(edge._tldraw_, yMax);
        }
    }

    public BoundingBox BB { get; }
    public IReadOnlyDictionary<string /* Name */, Cluster> Clusters { get; }
    public IReadOnlyDictionary<string /* Name */, Node> Nodes { get; }
    public IReadOnlyDictionary<string /* Id */, Edge> Edges { get; }

    public DeserializedGraphvizJsonOutput(GraphvizJsonOutput rawDeserializedGraph)
    {
        var yMax = (Pts)rawDeserializedGraph.bb!.Split(',')[3];
        BB = new BoundingBox(rawDeserializedGraph.bb, yMax);

        Nodes = rawDeserializedGraph.objects is null
                    ? []
                    : rawDeserializedGraph.objects.Where(o => !o.name!.StartsWith("cluster"))
                                                  .Select(n => new Node(n, yMax))
                                                  .ToDictionary(n => n.Name);

        Clusters = rawDeserializedGraph.objects is null 
                    ? []
                    : rawDeserializedGraph.objects.Where(o => o.name!.StartsWith("cluster") && o.bb != null)
                                                  .Select(c => new Cluster(c, yMax))
                                                  .ToDictionary(c => c.Name);

        Edges = rawDeserializedGraph.edges is null
                    ? []
                    : rawDeserializedGraph.edges.Select(e => new Edge(e, yMax))
                                                .ToDictionary(e => e.Id);
    }
}
