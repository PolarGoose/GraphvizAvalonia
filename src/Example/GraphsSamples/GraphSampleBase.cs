
using GraphvizAvalonia;

namespace Example.GraphsSamples;

internal abstract class GraphSampleBase
{
    public abstract string Name { get; }
    public abstract GraphDescription Description { get; }
    public override string ToString() => Name;
}
