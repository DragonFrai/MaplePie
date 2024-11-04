using MaplePie.Parser;

namespace MaplePie.Parsers.Combine;

public struct LazyParser<TParser, TToken, TOutput, TError>(Func<TParser> parserFactory)
    : IParser<LazyParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>
{
    private readonly Func<TParser> _parserFactory = parserFactory;

    public static 
        ParseResult<TToken, TOutput, TError> 
        Parse(
            ref LazyParser<TParser, TToken, TOutput, TError> self, 
            ReadOnlySpan<TToken> input, 
            int position)
    {
        var parser = self._parserFactory();
        return TParser.Parse(ref parser, input, position);
    }
}
