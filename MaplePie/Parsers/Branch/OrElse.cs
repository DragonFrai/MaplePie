using MaplePie.Parser;

namespace MaplePie.Parsers.Branch;

public struct OrElseParser<TParser1, TParser2, TToken, TOutput, TError>(TParser1 parser, TParser2 parserAlt)
    : IParser<OrElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser1 : IParser<TParser1, TToken, TOutput, TError>
    where TParser2 : IParser<TParser2, TToken, TOutput, TError>
{
    private TParser1 _parser = parser;
    private TParser2 _parserAlt = parserAlt;
    
    public static ParseResult<TToken, TOutput, TError> Parse(ref OrElseParser<TParser1, TParser2, TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        var result = TParser1.Parse(ref self._parser, input, position);
        return !result.IsMiss ? result : TParser2.Parse(ref self._parserAlt, input, position);
    }
}
