using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TToken"></typeparam>
public struct SkipSeqParser<TToken>(ReadOnlyMemory<TToken> tokens)
    : IParser<SkipSeqParser<TToken>, TToken, Unit, Unit>
{
    // Expected tokens sequence
    private readonly ReadOnlyMemory<TToken> _tokens = tokens;

    public static ParseResult<TToken, Unit, Unit> Parse(
        ref SkipSeqParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        return
            ParseFuncUtils.MoveManyEq(self._tokens.Span, input, ref position)
            ? ParseResult<TToken, Unit, Unit>.Ok(position, Unit.Instance)
            : ParseResult<TToken, Unit, Unit>.Miss(position, Unit.Instance);
    }
}
