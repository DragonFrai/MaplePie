using System.Diagnostics;

namespace MaplePie.Stuff;

public readonly struct PositionRange
{
    public readonly int Position;
    public readonly int Length;

    public PositionRange(int position, int length)
    {
        Position = position;
        Length = length;
    }
    
    public PositionRange(): this(0, 0) { }

    public PositionRange(Position begin, Position end)
    {
        var endOffset = begin.OffsetTo(end);
        
        if (endOffset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(end), $"Slice to previous cursor position.");
        }
        
        Position = begin.Pointer;
        Length = endOffset;
    }
    
    public Position Begin => new Position(Position);
    public Position End => new Position(Position + Length);

    public bool IsEmpty => Length == 0;
    
    public PositionRange Take(int count)
    {
        Trace.Assert(count >= 0 && count <= Length);
        return new PositionRange(Position, count);
    }

    public PositionRange Skip(int count)
    {
        Trace.Assert(count >= 0 && count <= Length);
        return new PositionRange(Position + count, Length - count);
    }

    public void Split(int count, out PositionRange left, out PositionRange right)
    {
        left = Take(count);
        right = Skip(count);
    }

    public PositionRange Limit(int maxLength)
    {
        return new PositionRange(Position, Math.Min(Length, maxLength));
    }

    public PositionRange Concat(PositionRange next)
    {
        if (Position + Length != next.Position)
        {
            throw new ArgumentException($"Next {nameof(PositionRange)} must start where this one ends.", nameof(next));
        }

        return new PositionRange(Position, Length + next.Length);
    }
}


public static class PositionReadOnlySpanExtensions
{
    public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> span, PositionRange positionRange)
    {
        return span.Slice(positionRange.Position, positionRange.Length);
    }

    public static ReadOnlySpan<T> Slice<T>(this ReadOnlySpan<T> span, Position position)
    {
        return span.Slice(position.Pointer);
    }
}

public static class PositionSpanExtensions
{
    public static Span<T> Slice<T>(this Span<T> span, PositionRange positionRange)
    {
        return span.Slice(positionRange.Position, positionRange.Length);
    }

    public static Span<T> Slice<T>(this Span<T> span, Position position)
    {
        return span.Slice(position.Pointer);
    }
}