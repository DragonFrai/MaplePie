using MaplePie.Parser;

namespace MaplePie.Parsers.Sequence;


public struct BetweenParser<TParserL, TParser, TParserR, TToken, TOutputL, TOutput, TOutputR, TError>(
    TParserL parserL,
    TParser parser,
    TParserR parserR)
    : IParser<BetweenParser<TParserL, TParser, TParserR, TToken, TOutputL, TOutput, TOutputR, TError>, TToken, TOutput,
        TError>
    where TParserL : IParser<TParserL, TToken, TOutputL, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>
    where TParserR : IParser<TParserR, TToken, TOutputR, TError>
{
    private TParserL _parserL = parserL;
    private TParser _parser = parser;
    private TParserR _parserR = parserR;
    
    public static ParseResult<TToken, TOutput, TError> Parse(ref BetweenParser<TParserL, TParser, TParserR, TToken, TOutputL, TOutput, TOutputR, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        var resultL = TParserL.Parse(ref self._parserL, input, position);
        if (!resultL.IsOk) { return resultL.Anything<TOutput>(); }
        position = resultL.Position;

        var result = TParser.Parse(ref self._parser, input, position);
        if (!result.IsOk) { return result.Anything<TOutput>(); }
        position = result.Position;

        var resultR = TParserR.Parse(ref self._parserR, input, position);
        if (!resultR.IsOk) { return resultR.Anything<TOutput>(); }
        position = resultR.Position;

        return ParseResult<TToken, TOutput, TError>.Ok(position, result.Output);
    }
}
