using MaplePie.Parser;

namespace MaplePie.Parsers.Sequence;


public struct Tuple2Parser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
    TParser1 parserL,
    TParser2 parserR)
    : IParser<Tuple2Parser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, ValueTuple<TOutput1, TOutput2>, TError>
    where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
    where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
{
    private TParser1 _parserL = parserL;
    private TParser2 _parserR = parserR;

    public static ParseResult<TToken, ValueTuple<TOutput1, TOutput2>, TError> Parse(ref Tuple2Parser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        var resultL = TParser1.Parse(ref self._parserL, input, position);
        if (!resultL.IsOk) { return resultL.Anything<ValueTuple<TOutput1, TOutput2>>(); }
        position = resultL.Position;

        var resultR = TParser2.Parse(ref self._parserR, input, position);
        if (!resultR.IsOk) { return resultR.Anything<ValueTuple<TOutput1, TOutput2>>(); }
        position = resultR.Position;

        var result = new ValueTuple<TOutput1, TOutput2>(resultL.Output, resultR.Output);
        return ParseResult<TToken, ValueTuple<TOutput1, TOutput2>, TError>.Ok(position, result);
    }
}
