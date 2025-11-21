using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace GraphvizAvalonia.Impl;

internal sealed class GraphvizDrawCommandsExecutor(Canvas canvas,
                                                   GraphDescription.Edge edgeDescription,
                                                   IEnumerable<DeserializedGraphvizJsonOutput.DrawCommand> drawCommands,
                                                   GraphDescription.Edge.LineStyleType lineStyle = GraphDescription.Edge.LineStyleType.Solid)
{
    private readonly Canvas canvas = canvas;
    private readonly IEnumerable<DeserializedGraphvizJsonOutput.DrawCommand> drawCommands = drawCommands;
    private readonly GraphDescription.Edge edgeDescription = edgeDescription;
    private readonly GraphDescription.Edge.LineStyleType lineStyle = lineStyle;
    private Color fillColor = Colors.Transparent;

    public void Execute()
    {
        if (drawCommands == null)
            return;

        foreach (var cmd in drawCommands)
            ExecuteDrawCommand(cmd);
    }

    private void ExecuteDrawCommand(DeserializedGraphvizJsonOutput.DrawCommand cmd)
    {
        switch (cmd)
        {
            case DeserializedGraphvizJsonOutput.DrawCommand.SetFillColor setFillColorCmd:
                fillColor = setFillColorCmd.FillColor == "#ffffff" ? Colors.Transparent : edgeDescription.Color;
                break;

            case DeserializedGraphvizJsonOutput.DrawCommand.EllipseDrawCommand ellipseCmd:
                DrawEllipse(ellipseCmd);
                break;

            case DeserializedGraphvizJsonOutput.DrawCommand.PolygonDrawCommand polygonCmd:
                DrawPolygon(polygonCmd);
                break;

            case DeserializedGraphvizJsonOutput.DrawCommand.PolyLineDrawCommand polyLineCmd:
                DrawPolyline(polyLineCmd);
                break;

            case DeserializedGraphvizJsonOutput.DrawCommand.BSpline splineCmd:
                DrawBSpline(splineCmd);
                break;

            case DeserializedGraphvizJsonOutput.DrawCommand.TextDraw textCmd:
                DrawText(textCmd);
                break;

            default:
                break;
        }
    }

    private void DrawEllipse(DeserializedGraphvizJsonOutput.DrawCommand.EllipseDrawCommand cmd)
    {
        var ellipse = new Ellipse
        {
            Width = cmd.W.Value * 2,
            Height = cmd.H.Value * 2,
            Stroke = new SolidColorBrush(edgeDescription.Color),
            StrokeThickness = edgeDescription.LineWidth,
            StrokeDashArray = StyleToStrokeDashArray(lineStyle),
            Fill = new SolidColorBrush(fillColor)
        };

        Canvas.SetLeft(ellipse, (cmd.Center.X - cmd.W).Value);
        Canvas.SetTop(ellipse, (cmd.Center.Y - cmd.H).Value);
        canvas.Children.Add(ellipse);
    }

    private void DrawPolygon(DeserializedGraphvizJsonOutput.DrawCommand.PolygonDrawCommand cmd)
    {
        canvas.Children.Add(new Polygon
        {
            Stroke = new SolidColorBrush(edgeDescription.Color),
            StrokeDashArray = StyleToStrokeDashArray(lineStyle),
            StrokeThickness = edgeDescription.LineWidth,
            Fill = new SolidColorBrush(fillColor),
            Points = [.. cmd.Points.Select(p => p)]
        });
    }

    private void DrawPolyline(DeserializedGraphvizJsonOutput.DrawCommand.PolyLineDrawCommand cmd)
    {
        canvas.Children.Add(new Polyline
        {
            Stroke = new SolidColorBrush(edgeDescription.Color),
            StrokeThickness = edgeDescription.LineWidth,
            StrokeDashArray = StyleToStrokeDashArray(lineStyle),
            Points = [.. cmd.Points.Select(p => p)]
        });
    }

    private void DrawText(DeserializedGraphvizJsonOutput.DrawCommand.TextDraw cmd)
    {
        var textBlock = new TextBlock
        {
            Text = cmd.Text,
            Foreground = new SolidColorBrush(edgeDescription.Color),
            FontSize = edgeDescription.FontSize,
            FontFamily = new FontFamily(edgeDescription.FontName)
        };

        Canvas.SetLeft(textBlock, (cmd.Center.X - cmd.TextWidth / 2).Value);
        Canvas.SetTop(textBlock, cmd.Center.Y.Value - edgeDescription.FontSize * 0.8);
        canvas.Children.Add(textBlock);
    }

    private void DrawBSpline(DeserializedGraphvizJsonOutput.DrawCommand.BSpline cmd)
    {
        var path = new Avalonia.Controls.Shapes.Path
        {
            Stroke = new SolidColorBrush(edgeDescription.Color),
            StrokeThickness = edgeDescription.LineWidth,
            StrokeDashArray = StyleToStrokeDashArray(lineStyle),
            Data = new PathGeometry
            {
                Figures = [
                    new PathFigure
                    {
                        StartPoint = cmd.StartPoint,
                        Segments = [.. cmd.Segments.Select(s => new BezierSegment { Point1 = s[0], Point2 = s[1], Point3 = s[2] })],
                        IsClosed = false
                    }
                ]
            }
        };

        canvas.Children.Add(path);
    }

    private AvaloniaList<double> StyleToStrokeDashArray(GraphDescription.Edge.LineStyleType s) => s switch
    {
        GraphDescription.Edge.LineStyleType.Dashed => [6, 2],
        GraphDescription.Edge.LineStyleType.Dotted => [1],
        GraphDescription.Edge.LineStyleType.Solid => [],
        _ => throw new ArgumentOutOfRangeException(nameof(s), $"Unsupported line drawing style: {s}")
    };
}
