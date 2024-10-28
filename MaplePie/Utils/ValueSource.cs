namespace MaplePie.Utils;


public struct ValueSource<T>
{
    private T? _value;
    private Func<T>? _func;

    public ValueSource()
    {
        const string msg = $"{nameof(ValueSource<T>)} can not be instantiated with primary constructor.";
        throw new InvalidOperationException(msg);
    }
    
    private ValueSource(T? value, Func<T>? func)
    {
        _value = value;
        _func = func;
    }

    public static ValueSource<T> Value(T value)
    {
        return new ValueSource<T>(value, null);
    }

    public static ValueSource<T> Func(Func<T> func)
    {
        return new ValueSource<T>(default, func);
    }

    public T Get()
    {
        return _func == null ? _value! : _func.Invoke();
    }
}
