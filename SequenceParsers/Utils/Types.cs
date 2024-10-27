using System.Diagnostics;

namespace SequenceParsers.Utils;


public readonly struct Unit()
{
    public static Unit Instance => new Unit();
}

public class Never
{
    private Never() {}

    public T Absurd<T>()
    {
        throw new UnreachableException($"{nameof(Never)} was instantiated.");
    }
}