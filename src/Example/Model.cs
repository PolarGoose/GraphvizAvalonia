using Avalonia.Media;
using Example.GraphsSamples;
using GraphvizAvalonia;

namespace Example;

internal sealed class NodeModel(string name)
{
    public string Name { get; } = name;
}

internal sealed class SubgraphModel(IBrush backgroundColor, IBrush strokeColor)
{
    public IBrush BackgroundColor { get; } = backgroundColor;
    public IBrush StrokeColor { get; } = strokeColor;
}

internal sealed class MainWindowModel
{
    public IEnumerable<GraphSampleBase> GraphSamples => [ new SimpleGraph(), new ComplexGraph() ];
    public IEnumerable<GraphvizLayoutEngine> LayoutEngine => Enum.GetValues(typeof(GraphvizLayoutEngine)).Cast<GraphvizLayoutEngine>();
}
