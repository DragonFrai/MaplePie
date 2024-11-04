using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


public struct SkipWhileParser<TToken>(Func<TToken, bool> condition)
    : IParser<SkipWhileParser<TToken>, TToken, Unit, Unit>
{
    private readonly Func<TToken, bool> _condition = condition;

    public static ParseResult<TToken, Unit, Unit> Parse(
        ref SkipWhileParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        ParseFuncUtils.MoveWhile(self._condition, input, ref position);
        return ParseResult<TToken, Unit, Unit>.Ok(position, Unit.Instance);
    }
}
