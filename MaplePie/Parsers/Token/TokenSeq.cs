using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


/// <summary>
/// Проверяет
/// </summary>
/// <typeparam name="TToken"></typeparam>
public struct TokenSeqParser<TToken>(ReadOnlyMemory<TToken> tokens)
    : IParser<TokenSeqParser<TToken>, TToken, ReadOnlyMemory<TToken>, Unit>
{
    // Expected tokens sequence
    private readonly ReadOnlyMemory<TToken> _tokens = tokens;

    public static 
        ParseResult<TToken, ReadOnlyMemory<TToken>, Unit> Parse
        (
            ref TokenSeqParser<TToken> self,
            ReadOnlySpan<TToken> input, 
            int position
        )
    {
        return
            ParseFuncUtils.MoveManyEq(self._tokens.Span, input, ref position)
            ? ParseResult<TToken, ReadOnlyMemory<TToken>, Unit>.Ok(position, self._tokens)
            : ParseResult<TToken, ReadOnlyMemory<TToken>, Unit>.Miss(position, Unit.Instance);
    }
}
