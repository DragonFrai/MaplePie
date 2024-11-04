using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

public struct TokenEqParser<TToken> 
    : IParser<TokenEqParser<TToken>, TToken, TToken, Unit>
{
    private TToken _comparand;

    public TokenEqParser(TToken comparand)
    {
        _comparand = comparand;
    }

    public static ParseResult<TToken, TToken, Unit> Parse(
        ref TokenEqParser<TToken> self,
        ReadOnlySpan<TToken> input, 
        int position)
    {
        var beginPosition = position;
        return
            ParseFuncUtils.MoveOneEq(self._comparand, input, ref position)
                ? ParseResult<TToken, TToken, Unit>.Ok(position, input[beginPosition])
                : ParseResult<TToken, TToken, Unit>.Miss(position, Unit.Instance);
    }
}
