using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


public struct TokenIsParser<TToken>
    : IParser<TokenIsParser<TToken>, TToken, TToken, Unit>
{
    private Func<TToken, bool> _condition;

    public TokenIsParser(Func<TToken, bool> condition)
    {
        _condition = condition;
    }

    public static
        ParseResult<TToken, TToken, Unit> 
        Parse
        (
            ref TokenIsParser<TToken> self,
            ReadOnlySpan<TToken> input,
            int position
        )
    {
        var beginPosition = position;
        return
            ParseFuncUtils.MoveOneIf(self._condition, input, ref position)
                ? ParseResult<TToken, TToken, Unit>.Ok(position, input[beginPosition])
                : ParseResult<TToken, TToken, Unit>.Miss(position, Unit.Instance);
    }
}