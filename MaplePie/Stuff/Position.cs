using System.Diagnostics;

namespace MaplePie.Stuff;


public readonly struct Position(int pointer)
{
    public readonly int Pointer = pointer;

    public Position(): this(0) { }

    public Position Move(int offset)
    {
        return new Position(Pointer + offset);
    }

    public int OffsetTo(Position end)
    {
        return end.Pointer - Pointer;
    }
}

