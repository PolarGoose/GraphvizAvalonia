namespace GraphvizAvalonia;

public static class GraphvizBinariesLocation
{
    public static void Set(string folderWithGraphvizBinaries)
    {
        Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + Path.PathSeparator + folderWithGraphvizBinaries);
    }
}
