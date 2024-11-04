using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Like Parse.Consumed(Parse.SkipSeq(tokens))
/// </remarks>
/// <param name="tokens"></param>
/// <typeparam name="TToken"></typeparam>
public struct TakeSeqParser<TToken>(ReadOnlyMemory<TToken> tokens)
    : IParser<TakeSeqParser<TToken>, TToken, InputRange, Unit>
{
    // Expected tokens sequence
    private readonly ReadOnlyMemory<TToken> _tokens = tokens;

    public static ParseResult<TToken, InputRange, Unit> Parse(
        ref TakeSeqParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        var beginPosition = position;
        if (!ParseFuncUtils.MoveManyEq(self._tokens.Span, input, ref position))
            return ParseResult<TToken, InputRange, Unit>.Miss(position, Unit.Instance);
        var range = new InputRange(beginPosition, position - beginPosition);
        return ParseResult<TToken, InputRange, Unit>.Ok(position, range);
    }
}

