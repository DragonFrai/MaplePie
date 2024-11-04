using MaplePie.Parser;

namespace MaplePie.Parsers.Sequence;


public struct
    PairLeftParser<TParserL, TParserR, TToken, TOutputL, TOutputR, TError>(TParserL parserL, TParserR parserR)
    : IParser<PairLeftParser<TParserL, TParserR, TToken, TOutputL, TOutputR, TError>, TToken, TOutputL, TError>
    where TParserL : IParser<TParserL, TToken, TOutputL, TError>
    where TParserR : IParser<TParserR, TToken, TOutputR, TError>
{
    private TParserL _parserL = parserL;
    private TParserR _parserR = parserR;

    public static
        ParseResult<TToken, TOutputL, TError> Parse(
            ref PairLeftParser<TParserL, TParserR, TToken, TOutputL, TOutputR, TError> self,
            ReadOnlySpan<TToken> input,
            int position)
    {
        var resultL = TParserL.Parse(ref self._parserL, input, position);
        if (!resultL.IsOk) { return resultL.Anything<TOutputL>(); }

        var resultR = TParserR.Parse(ref self._parserR, input, resultL.Position);
        if (!resultR.IsOk) { return resultR.Anything<TOutputL>(); }

        return ParseResult<TToken, TOutputL, TError>.Ok(resultR.Position, resultL.Output);
    }
}
