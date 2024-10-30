using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.CombineParsers;


public struct EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError> 
    : IParser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>, TToken, TOutput, TError>
    where TParser: IParser<TParser, TToken, TOutput, TError>
    where TEndParser: IParser<TEndParser, TToken, TEndOutput, TError>
{
    private TParser _parser;
    private TEndParser _endParser;

    public EndsWithParser(TParser parser, TEndParser endParser)
    {
        _parser = parser;
        _endParser = endParser;
    }

    public static
        ParseResult<TToken, TOutput, TError> Parse(
            ref EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError> self,
            ReadOnlySpan<TToken> input,
            int range)
    {
        var firstR = TParser.Parse(ref self._parser, input, range);
        if (!firstR.IsOk) { return firstR.Anything<TOutput>(); }

        var endR = TEndParser.Parse(ref self._endParser, input, firstR.Position);
        if (!endR.IsOk) { return endR.Anything<TOutput>(); }

        return ParseResult<TToken, TOutput, TError>.Ok(endR.Position, firstR.Output);
    }
}
