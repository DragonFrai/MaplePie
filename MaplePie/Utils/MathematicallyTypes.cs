using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MaplePie.Utils;


public readonly struct Unit : IStructuralEquatable, IComparable, IStructuralComparable
{
    public static readonly Unit Instance = new();
    
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Unit;
        }

        public bool Equals(Unit other)
        {
            return true;
        }

        bool IStructuralEquatable.Equals(object? other, IEqualityComparer comparer)
        {
            return other is Unit;
        }

        int IComparable.CompareTo(object? other)
        {
            if (other is null) return 1;

            if (other is not Unit)
            {
                throw new Exception("Unit not comparable with other types");
            }

            return 0;
        }
        
        public int CompareTo(Unit other)
        {
            return 0;
        }

        int IStructuralComparable.CompareTo(object? other, IComparer comparer)
        {
            if (other is null) return 1;

            if (other is not Unit)
            {
                throw new Exception("Unit not comparable with other types");
            }

            return 0;
        }
        
        public override int GetHashCode()
        {
            return 0;
        }

        int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
        {
            return 0;
        }
        
        public override string ToString()
        {
            return "()";
        }
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
