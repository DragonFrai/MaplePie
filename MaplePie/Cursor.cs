using System.Diagnostics;

namespace MaplePie;


public readonly struct Cursor(int position)
{
    public readonly int Position = position;

    public Cursor(): this(0) { }

    public Cursor Move(int offset)
    {
        return new Cursor(Position + offset);
    }

    public int OffsetTo(Cursor end)
    {
        return end.Position - Position;
    }
}

public readonly struct CursorRange
{
    public readonly int Position;
    public readonly int Length;

    public CursorRange(int position, int length)
    {
        Position = position;
        Length = length;
    }
    
    public CursorRange(): this(0, 0) { }

    public CursorRange(Cursor begin, Cursor end)
    {
        var endOffset = begin.OffsetTo(end);
        
        if (endOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(end), $"Slice to previous cursor position.");
        }
        
        Position = begin.Position;
        Length = endOffset;
    }
    
    public Cursor Begin => new Cursor(Position);
    public Cursor End => new Cursor(Position + Length);

    public bool IsEmpty => Length == 0;
    
    public CursorRange Take(int count)
    {
        Trace.Assert(count >= 0 && count <= Length);
        return new CursorRange(Position, count);
    }

    public CursorRange Skip(int count)
    {
        Trace.Assert(count >= 0 && count <= Length);
        return new CursorRange(Position + count, Length - count);
    }

    public void Split(int count, out CursorRange left, out CursorRange right)
    {
        left = Take(count);
        right = Skip(count);
    }
}


public static class ReadOnlySpanExtensions
{
    public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> span, CursorRange cursorRange)
    {
        return span.Slice(cursorRange.Position, cursorRange.Length);
    }

    public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> span, Cursor cursor)
    {
        return span.Slice(cursor.Position);
    }
}

public static class SpanExtensions
{
    public static Span<T> Slice<T>(this Span<T> span, CursorRange cursorRange)
    {
        return span.Slice(cursorRange.Position, cursorRange.Length);
    }

    public static Span<T> Slice<T>(this Span<T> span, Cursor cursor)
    {
        return span.Slice(cursor.Position);
    }
}