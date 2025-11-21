using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GraphvizAvalonia.Impl;

#pragma warning disable CS0649

// All fields with "double" type have units in points (1/72 inch) unless specified otherwise.
[DataContract]
internal class GraphvizJsonOutput
{
    [DataContract]
    internal class DrawCommand
    {
        [DataMember] public string? op; // operation code like "E", "P", "T", etc. All codes are described in https://graphviz.org/docs/outputs/canon/#xdot
        [DataMember] public string? color; // only for "C" and "c" operations
        [DataMember] public double[][/* 2 */]? points; // array of points.
        [DataMember] public double[/* 4 */]? rect; // only for "E" and "e" operations. Ellipse specification { x-center, y-center, x-radius, y-radius }
        [DataMember] public double[/* 2 */]? pt; // only for "T" operation. Text position. By default the text alignment is "centered" and thus x and y
                                                // coordinate are the center of the text for x and y axis respectively (at the center of the text box)
        [DataMember] public double width; // only for "T" operation. With of the Text.
        [DataMember] public string? text; // only for "T" operation. Text to draw.
    }

    [DataContract]
    internal class Edge
    {
        [DataMember] public string? id;
        [DataMember] public DrawCommand[]? _draw_; // edge curve
        [DataMember] public DrawCommand[]? _hdraw_; // head arrowhead
        [DataMember] public DrawCommand[]? _hldraw_; // label next to the head arrowhead
        [DataMember] public DrawCommand[]? _ldraw_; // label on the edge (usually in the middle)
        [DataMember] public DrawCommand[]? _tdraw_; // tail arrowhead
        [DataMember] public DrawCommand[]? _tldraw_; // label next to the tail arrowhead
    }

    [DataContract]
    internal class ClusterOrNode
    {
        [DataMember] public string? name; // Cluster or Node name
        [DataMember] public string? pos; // center position. Only for nodes
        [DataMember] public double? height; // only for nodes. Units: inches
        [DataMember] public double? width; // only for nodes. Units: inches
        [DataMember] public string? bb; // bounding box. Only for clusters
    }

    [DataMember] public string? bb; // bounding box
    [DataMember] public ClusterOrNode[]? objects;
    [DataMember] public Edge[]? edges;

    static public GraphvizJsonOutput Deserialize(string graphVizJsonOutput)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(graphVizJsonOutput));
        return (GraphvizJsonOutput)new DataContractJsonSerializer(typeof(GraphvizJsonOutput)).ReadObject(stream);
    }
}
