namespace MaplePie;



public readonly struct InputRange(CursorRange cursorRange)
{
    public readonly CursorRange CursorRange = cursorRange;
    
    public InputRange(): this(new CursorRange()) { }
    public InputRange(int position, int length): this(new CursorRange(position, length)) { }

    public int Position => CursorRange.Position;
    public int Length => CursorRange.Length;

    public Cursor Begin => CursorRange.Begin;
    public Cursor End => CursorRange.End;
    public bool IsEmpty => CursorRange.IsEmpty;

    public InputRange Take(int count)
    {
        return new InputRange(CursorRange.Take(count));
    }
    
    public InputRange Skip(int count)
    {
        return new InputRange(CursorRange.Skip(count));
    }
    
    public void Split(int count, out InputRange left, out InputRange right)
    {
        CursorRange.Split(count, out var leftRange, out var rightRange);
        left = new InputRange(leftRange);
        right = new InputRange(rightRange);
    }
}

/// <summary>
/// Struct-based parser interface.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TError"></typeparam>
public interface IParser<TSelf, TToken, TOutput, TError> where TSelf: IParser<TSelf, TToken, TOutput, TError>
{
    // TODO: Compose (ReadOnlySpan<TToken> input, InputRange range) into one structure.
    public static abstract ParseResult<TToken, TOutput, TError> Parse(ref TSelf self, ReadOnlySpan<TToken> input, InputRange range);
}

/// <summary>
/// Struct-based parser implementation wrapper.
/// Designed for more easy method invocations (Without specify all generics in extensions methods).
/// Any struct-based parser should be wrapped on it before getting into the user code.
/// </summary>
/// <param name="parser"></param>
/// <typeparam name="TParser"></typeparam>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TError"></typeparam>
public struct Parser<TParser, TToken, TOutput, TError>(TParser parser)
    where TParser : struct, IParser<TParser, TToken, TOutput, TError>
{
    public TParser Inner = parser;
}

public static class FundamentalParserExtensions
{
    public static ParseResult<TToken, TOutput, TError> Parse<TParser, TToken, TOutput, TError>(
        this ref Parser<TParser, TToken, TOutput, TError> parser, ReadOnlySpan<TToken> input, InputRange range) where TParser: struct, IParser<TParser, TToken, TOutput, TError>
    {
        return TParser.Parse(ref parser.Inner, input, range);
    }

    public static ParseResult<TToken, TOutput, TError> BeginParse<TParser, TToken, TOutput, TError>(
        this ref Parser<TParser, TToken, TOutput, TError> parser, ReadOnlySpan<TToken> input) where TParser: struct, IParser<TParser, TToken, TOutput, TError>
    {
        return TParser.Parse(ref parser.Inner, input, new InputRange(0, input.Length));
    }
}
