using System.Diagnostics;

namespace MaplePie.Parser;


public readonly struct InputRange
{
    public readonly int Position;
    public readonly int Length;

    public InputRange(int position, int length)
    {
        Position = position;
        Length = length;
    }
    
    public InputRange(): this(0, 0) { }

    public int Begin => Position;
    public int End => Position + Length;

    public bool IsEmpty => Length == 0;
    
    public InputRange Take(int count)
    {
        Trace.Assert(count >= 0 && count <= Length);
        return new InputRange(Position, count);
    }

    public InputRange Skip(int count)
    {
        Trace.Assert(count >= 0 && count <= Length);
        return new InputRange(Position + count, Length - count);
    }

    public void Split(int count, out InputRange left, out InputRange right)
    {
        Trace.Assert(count >= 0 && count <= Length);
        left = new InputRange(Position, count);
        right = new InputRange(Position + count, Length - count);
    }

    public InputRange Limit(int maxLength)
    {
        return new InputRange(Position, Math.Min(Length, maxLength));
    }
    
}
