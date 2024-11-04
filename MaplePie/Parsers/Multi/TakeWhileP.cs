using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.Parsers.Multi;

public struct TakeWhilePParser<TCondParser, TToken, TCondOutput, TCondError> 
    : IParser<TakeWhilePParser<TCondParser, TToken, TCondOutput, TCondError>, TToken, InputRange, TCondError>
    where TCondParser : IParser<TCondParser, TToken, TCondOutput, TCondError>
{
    private TCondParser _condition;

    public TakeWhilePParser(TCondParser condition)
    {
        _condition = condition;
    }

    public static
        ParseResult<TToken, InputRange, TCondError> Parse(
            ref TakeWhilePParser<TCondParser, TToken, TCondOutput, TCondError> self,
            ReadOnlySpan<TToken> input,
            int position)
    {
        var takedPosition = position;
        while (true)
        {
            var result = TCondParser.Parse(ref self._condition, input, takedPosition);
            var resultKind = result.Kind;
            switch (resultKind)
            {
                case ParseResultKind.Ok:
                    takedPosition = result.Position;
                    break;
                case ParseResultKind.Miss:
                {
                    var resultRange = new InputRange(position, takedPosition - position);
                    return ParseResult<TToken, InputRange, TCondError>.Ok(takedPosition, resultRange);
                }
                case ParseResultKind.Fail:
                    return ParseResult<TToken, InputRange, TCondError>.Fail(result.Position, result.Error);
                default:
                    throw new UnreachableException();
            }
        }
    }
}