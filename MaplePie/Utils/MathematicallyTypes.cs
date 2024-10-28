using System.Diagnostics;

namespace MaplePie.Utils;


public readonly struct Unit
{
    public static readonly Unit Instance = new Unit();
}


#pragma warning disable CA2231

public struct Never
{
    [Obsolete("Never must never be initialized.", true)]
    public Never()
    {
        throw new InvalidOperationException($"{nameof(Never)} must never be initialized.");
    }

    private static T ThrowAbsurd<T>()
    {
        throw new UnreachableException($"{nameof(Never)} was instantiated.");
    }
    
    public T Absurd<T>()
    {
        return ThrowAbsurd<T>();
    }

    public override string ToString()
    {
        return ThrowAbsurd<string>();
    }

    public override bool Equals(object? obj)
    {
        return ThrowAbsurd<bool>();
    }

    public override int GetHashCode()
    {
        return ThrowAbsurd<int>();
    }
}

#pragma warning restore CA2231
