namespace MaplePie.Utils;


public interface IOption<TSelf, T>
{
    // TODO? Not use ref ?
    public static abstract T Value(ref TSelf self);
    public static abstract bool HasValue(ref TSelf self);
    public static abstract TSelf None();
    public static abstract TSelf Some(T value);
}

public class NullableOptionProxy<T> : IOption<T?, T> where T : class
{
    public static T Value(ref T? self)
    {
        if (HasValue(ref self))
        {
            return self!;
        }
        throw new NullReferenceException("Value is null");
    }

    public static bool HasValue(ref T? self)
    {
        return self is not null;
    }

    public static T? None()
    {
        return null;
    }

    public static T? Some(T value)
    {
        return value;
    }
}

public class ValueNullableOptionProxy<T> : IOption<T?, T> where T : struct
{
    public static T Value(ref T? self)
    {
        if (HasValue(ref self))
        {
            return (T)self!;
        }
        throw new NullReferenceException("Value is null");
    }

    public static bool HasValue(ref T? self)
    {
        return self is not null;
    }

    public static T? None()
    {
        return null;
    }

    public static T? Some(T value)
    {
        return value;
    }
}
