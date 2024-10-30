namespace MaplePie.Utils;

public static class SpanExtensions
{
    public static void Split<T>(this Span<T> span, int position, out Span<T> left, out Span<T> right)
    {
        left = span[..position];
        right = span[position..];
    }
}

public static class ReadOnlySpanExtensions
{
    public static void Split<T>(this ReadOnlySpan<T> span, int position, out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
    {
        left = span[..position];
        right = span[position..];
    }
}
