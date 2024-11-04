using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Like Parse.Consumed(Parse.SkipIf(condition))
/// </remarks>
/// <param name="condition"></param>
/// <typeparam name="TToken"></typeparam>
public struct TakeIfParser<TToken>(Func<TToken, bool> condition)
    : IParser<TakeIfParser<TToken>, TToken, InputRange, Unit>
{
    private readonly Func<TToken, bool> _condition = condition;

    public static ParseResult<TToken, InputRange, Unit> Parse(
        ref TakeIfParser<TToken> self,
        ReadOnlySpan<TToken> input,
        int position)
    {
        var beginPosition = position;
        if (ParseFuncUtils.MoveOneIf(self._condition, input, ref position))
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
