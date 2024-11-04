using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

// skip if cond
public struct SkipIfParser<TToken>(Func<TToken, bool> condition)
    : IParser<SkipIfParser<TToken>, TToken, Unit, Unit>
{
    private readonly Func<TToken, bool> _condition = condition;

    public static ParseResult<TToken, Unit, Unit> Parse(
        ref SkipIfParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        return
            ParseFuncUtils.MoveOneIf(self._condition, input, ref position)
            ? ParseResult<TToken, Unit, Unit>.Ok(position, Unit.Instance)
            : ParseResult<TToken, Unit, Unit>.Miss(position, Unit.Instance);
    }
}
