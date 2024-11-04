using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.Parsers.Combine;



public struct BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>
    : IParser<BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
    where TParser1: IParser<TParser1, TToken, TOutput1, TError>
    where TParser2: IParser<TParser2, TToken, TOutput2, TError>
{
    private TParser1 _parser1;
    private Func<TOutput1, TParser2> _binder1;

    public BindParser(
        TParser1 parserBox,
        Func<TOutput1, TParser2> binder)
    {
        _parser1 = parserBox;
        _binder1 = binder;
    }

    public static ParseResult<TToken, TOutput2, TError> Parse(
        ref BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError> self, ReadOnlySpan<TToken> input,
        int range)
    {
        var result1 = TParser1.Parse(ref self._parser1, input, range);
        switch (result1.Kind)
        {
            case ParseResultKind.Ok:
                var parser2 = self._binder1(result1.Output);
                var result2 = TParser2.Parse(ref parser2, input, result1.Position);
                return result2;
            case ParseResultKind.Miss:
                return ParseResult<TToken, TOutput2, TError>.Miss(result1.Position, result1.Error);
            case ParseResultKind.Fail:
                return ParseResult<TToken, TOutput2, TError>.Fail(result1.Position, result1.Error);
            default: throw new UnreachableException();
        }
    }
}
