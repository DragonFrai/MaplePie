using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

public struct TokenParser<TToken> 
    : IParser<TokenParser<TToken>, TToken, TToken, Unit>
{
    public TokenParser()
    {}

    public static ParseResult<TToken, TToken, Unit> Parse(
        ref TokenParser<TToken> self,
        ReadOnlySpan<TToken> input, 
        int position)
    {
        var beginPosition = position;
        return
            ParseFuncUtils.MoveOne(input, ref position)
                ? ParseResult<TToken, TToken, Unit>.Ok(position, input[beginPosition])
                : ParseResult<TToken, TToken, Unit>.Miss(position, Unit.Instance);
    }
}
