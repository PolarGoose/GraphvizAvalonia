namespace GraphvizAvalonia.Impl;

internal static class LinqExtensions
{
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }

    public static IEnumerable<T[]> Chunk<T>(this IEnumerable<T> source, int size)
    {
        using var e = source.GetEnumerator();
        while (e.MoveNext())
        {
            var chunk = new List<T>(size) { e.Current };
            for (int i = 1; i < size && e.MoveNext(); i++)
            {
                chunk.Add(e.Current);
            }
            yield return chunk.ToArray();
        }
    }
}
