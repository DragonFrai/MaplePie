using MaplePie.Utils;

namespace MaplePie.Combinators;


// TODO:? Generalize TEndOutput instead Unit type require ?
public struct EndsWithParser<TParser, TEndParser, TToken, TOutput, TError> 
    : IParser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser: IParser<TParser, TToken, TOutput, TError>
    where TEndParser: IParser<TEndParser, TToken, Unit, TError>
{
    private TParser _parser;
    private TEndParser _endParser;

    public EndsWithParser(TParser parser, TEndParser endParser)
    {
        _parser = parser;
        _endParser = endParser;
    }

    public static 
        ParseResult<TToken, TOutput, TError> 
        Parse(
            ref EndsWithParser<TParser, TEndParser, TToken, TOutput, TError> self, 
            ReadOnlySpan<TToken> input, 
            InputRange range)
    {
        var firstR = TParser.Parse(ref self._parser, input, range);
        if (!firstR.IsOk) { return firstR.Anything<TOutput>(); }

        var endR = TEndParser.Parse(ref self._endParser, input, firstR.RemainderRange);
        if (!endR.IsOk) { return endR.Anything<TOutput>(); }

        return ParseResult<TToken, TOutput, TError>.Ok(endR.RemainderRange, firstR.Output);
    }
}
