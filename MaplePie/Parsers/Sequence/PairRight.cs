using MaplePie.Parser;

namespace MaplePie.Parsers.Sequence;


public struct
    PairRightParser<TParserL, TParserR, TToken, TOutputL, TOutputR, TError>(TParserL parserL, TParserR parserR)
    : IParser<PairRightParser<TParserL, TParserR, TToken, TOutputL, TOutputR, TError>, TToken, TOutputR, TError>
    where TParserL : IParser<TParserL, TToken, TOutputL, TError>
    where TParserR : IParser<TParserR, TToken, TOutputR, TError>
{
    private TParserL _parserL = parserL;
    private TParserR _parserR = parserR;

    public static
        ParseResult<TToken, TOutputR, TError> Parse(
            ref PairRightParser<TParserL, TParserR, TToken, TOutputL, TOutputR, TError> self,
            ReadOnlySpan<TToken> input,
            int position)
    {
        var firstR = TParserL.Parse(ref self._parserL, input, position);
        if (!firstR.IsOk) { return firstR.Anything<TOutputR>(); }

        var endR = TParserR.Parse(ref self._parserR, input, firstR.Position);
        if (!endR.IsOk) { return endR.Anything<TOutputR>(); }

        return ParseResult<TToken, TOutputR, TError>.Ok(endR.Position, endR.Output);
    }
}