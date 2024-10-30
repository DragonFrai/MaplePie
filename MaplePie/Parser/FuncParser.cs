namespace MaplePie.Parser;


public delegate ParseResult<TToken, TOutVal, TError> ParseFunc<TToken, TOutVal, TError>(ReadOnlySpan<TToken> input, int position);

public struct FuncParser<TToken, TOutVal, TError>(ParseFunc<TToken, TOutVal, TError> func)
    : IParser<FuncParser<TToken, TOutVal, TError>, TToken, TOutVal, TError>
{
    private ParseFunc<TToken, TOutVal, TError> _func = func;

    public static ParseResult<TToken, TOutVal, TError> Parse(ref FuncParser<TToken, TOutVal, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        return self._func(input, position);
    }
}

public static class FuncParser
{
    public static Parser<FuncParser<TToken, TOutVal, TError>, TToken, TOutVal, TError>
        OfFunc<TToken, TOutVal, TError>(this ParseFunc<TToken, TOutVal, TError> func)
    {
        return new Parser<FuncParser<TToken, TOutVal, TError>, TToken, TOutVal, TError>(new FuncParser<TToken, TOutVal, TError>(func));
    }
}

public static class ParseFuncExtensions
{
    public static Parser<FuncParser<TToken, TOutVal, TError>, TToken, TOutVal, TError>
        ToParser<TToken, TOutVal, TError>(this ParseFunc<TToken, TOutVal, TError> func)
    {
        return new Parser<FuncParser<TToken, TOutVal, TError>, TToken, TOutVal, TError>(new FuncParser<TToken, TOutVal, TError>(func));
    }
}
