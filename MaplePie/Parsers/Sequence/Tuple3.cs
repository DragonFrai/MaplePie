using MaplePie.Parser;

namespace MaplePie.Parsers.Sequence;


public struct Tuple3Parser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>(
    TParser1 parserL,
    TParser2 parser,
    TParser3 parserR)
    : IParser<Tuple3Parser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>, TToken, ValueTuple<TOutput1, TOutput2, TOutput3>,
        TError>
    where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
    where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
    where TParser3 : IParser<TParser3, TToken, TOutput3, TError>
{
    private TParser1 _parserL = parserL;
    private TParser2 _parser = parser;
    private TParser3 _parserR = parserR;

    public static ParseResult<TToken, (TOutput1, TOutput2, TOutput3), TError> Parse(ref Tuple3Parser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        var result1 = TParser1.Parse(ref self._parserL, input, position);
        if (!result1.IsOk) { return result1.Anything<(TOutput1, TOutput2, TOutput3)>(); }
        position = result1.Position;

        var result2 = TParser2.Parse(ref self._parser, input, position);
        if (!result2.IsOk) { return result2.Anything<(TOutput1, TOutput2, TOutput3)>(); }
        position = result2.Position;

        var result3 = TParser3.Parse(ref self._parserR, input, position);
        if (!result3.IsOk) { return result3.Anything<(TOutput1, TOutput2, TOutput3)>(); }
        position = result2.Position;
        
        var result = new ValueTuple<TOutput1, TOutput2, TOutput3>(result1.Output, result2.Output, result3.Output);
        return ParseResult<TToken, (TOutput1, TOutput2, TOutput3), TError>.Ok(position, result);
    }
}
