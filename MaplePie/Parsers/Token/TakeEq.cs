using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Like Parse.Consumed(Parse.SkipEq(token))
/// </remarks>
/// <param name="token"></param>
/// <typeparam name="TToken"></typeparam>
public struct TakeEqParser<TToken>(TToken token)
    : IParser<TakeEqParser<TToken>, TToken, InputRange, Unit>
{
    private readonly TToken _token = token;

    public static ParseResult<TToken, InputRange, Unit> Parse(
        ref TakeEqParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        var beginPosition = position;
        if (ParseFuncUtils.MoveOneEq(self._token, input, ref position))
        {
            var range = new InputRange(beginPosition, 1);
            return ParseResult<TToken, InputRange, Unit>.Ok(position, range);
        }
        else
        {
            return ParseResult<TToken, InputRange, Unit>.Miss(position, Unit.Instance);
        }
    }
}