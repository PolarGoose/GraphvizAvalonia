using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Text;

namespace GraphvizAvalonia.Impl;

internal static class GraphvizInterop
{
    private sealed class SafeGraphHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeGraphHandle() : base(true) { }
        protected override bool ReleaseHandle() { agclose(handle); return true; }
    }

    private sealed class SafeContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeContextHandle() : base(true) { }
        protected override bool ReleaseHandle() { gvFreeContext(handle); return true; }
    }

    private sealed class SafeRenderDataHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeRenderDataHandle() : base(true) { }
        protected override bool ReleaseHandle() { gvFreeRenderData(handle); return true; }
    }

    private sealed class FreeLayoutAction(SafeContextHandle context, SafeGraphHandle graph) : IDisposable
    {
        public void Dispose() => gvFreeLayout(context, graph);
    }

    [DllImport("cgraph", CallingConvention = CallingConvention.Cdecl)]
    private static extern SafeGraphHandle agmemread(byte[] graphVizData);

    [DllImport("cgraph", CallingConvention = CallingConvention.Cdecl)]
    private static extern void agclose(IntPtr g);

    [DllImport("cgraph", CallingConvention = CallingConvention.Cdecl)]
    private static extern string aglasterr();

    [DllImport("gvc", CallingConvention = CallingConvention.Cdecl)]
    private static extern SafeContextHandle gvContext();

    [DllImport("gvc", CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvLayout(SafeContextHandle context, SafeGraphHandle graph, string engine);

    [DllImport("gvc", CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvFreeLayout(SafeContextHandle context, SafeGraphHandle graph);

    [DllImport("gvc", CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvRenderData(SafeContextHandle context, SafeGraphHandle graph, string format, out SafeRenderDataHandle result, out int length);

    [DllImport("gvc", CallingConvention = CallingConvention.Cdecl)]
    private static extern void gvFreeRenderData(IntPtr buffer);

    [DllImport("gvc", CallingConvention = CallingConvention.Cdecl)]
    private static extern int gvFreeContext(IntPtr gvc);

    public static string GenerateJsonOutput(string graphDescriptionInDotLanguage, GraphvizLayoutEngine layoutEngine)
    {
        using var graph = agmemread(Encoding.UTF8.GetBytes(graphDescriptionInDotLanguage + '\0'));
        if (graph.IsInvalid)
            throw new ArgumentException($"Unable to read the given graph description\naglasterr: {aglasterr()}\nGraph description:\n{graphDescriptionInDotLanguage}");

        using var context = gvContext();
        if (context.IsInvalid)
            throw new InvalidOperationException($"Unable to create the gvContext");

        if (gvLayout(context, graph, layoutEngine.ToString().ToLower()) != 0)
            throw new ArgumentException($"Unable to layout graph using layoutEngine={layoutEngine.ToString().ToLower()}\nGraph description:\n{graphDescriptionInDotLanguage}");

        using var _ = new FreeLayoutAction(context, graph);

        if (gvRenderData(context, graph, "json", out var renderBuffer, out var length) != 0)
            throw new ArgumentException($"Unable to generate json rendering of the graph\nGraph description:\n{graphDescriptionInDotLanguage}");
        
        using (renderBuffer)
        {
            var data = new byte[length];
            Marshal.Copy(renderBuffer.DangerousGetHandle(), data, 0, length);
            return Encoding.UTF8.GetString(data);
        }
    }
}
