using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


// skip one if eq
public struct SkipEqParser<TToken>(TToken token)
    : IParser<SkipEqParser<TToken>, TToken, Unit, Unit>
{
    private readonly TToken _token = token;

    public static ParseResult<TToken, Unit, Unit> Parse(
        ref SkipEqParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        return
            ParseFuncUtils.MoveOneEq(self._token, input, ref position)
            ? ParseResult<TToken, Unit, Unit>.Ok(position, Unit.Instance)
            : ParseResult<TToken, Unit, Unit>.Miss(position, Unit.Instance);
    }
}
