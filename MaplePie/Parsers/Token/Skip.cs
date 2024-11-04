using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

/// <summary>
/// Skip by count
/// </summary>
/// <typeparam name="TToken"></typeparam>
public struct SkipParser<TToken>(int count) : IParser<SkipParser<TToken>, TToken, Unit, Unit>
{
    private readonly int _count = count;

    public static ParseResult<TToken, Unit, Unit> Parse(
        ref SkipParser<TToken> self,
        ReadOnlySpan<TToken> input, int position)
    {
        return
            ParseFuncUtils.MoveN(self._count, input, ref position)
            ? ParseResult<TToken, Unit, Unit>.Ok(position, Unit.Instance)
            : ParseResult<TToken, Unit, Unit>.Miss(position, Unit.Instance);
    }
}
