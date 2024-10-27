namespace SequenceParsers;


public struct Needed
{
    private readonly int _count;

    public bool IsUnknown => _count == 0;

    public int Count
    {
        get
        {
            if (IsUnknown)
            {
                throw new InvalidOperationException("Unknown Needed can not contains count");
            }
            return Count;
        }
    }

    private Needed(int count)
    {
        if (count < 0)
        {
            throw new ArgumentException("Needed must be positive or 0 if unknown", nameof(count));
        }
        _count = count;
    }

    public static Needed Known(int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Known needed count must be > 0", nameof(count));
        }

        return new Needed(count);
    }

    public static readonly Needed Unknown = new Needed(-1);

    public override string ToString()
    {
        return _count == 0 ? "Unknown" : $"{_count}";
    }
}
