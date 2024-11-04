using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


/// <summary>
/// 
/// </summary>
/// <remarks>
/// Like Parse.Consumed(Parse.SkipWhile(condition))
/// </remarks>
/// <param name="condition"></param>
/// <typeparam name="TToken"></typeparam>
public struct TakeWhileParser<TToken>(Func<TToken, bool> condition)
    : IParser<TakeWhileParser<TToken>, TToken, InputRange, Unit>
{
    private Func<TToken, bool> _condition = condition;

    public static
        ParseResult<TToken, InputRange, Unit> Parse(ref TakeWhileParser<TToken> self,
            ReadOnlySpan<TToken> input,
            int position)
    {
        var satisfiedPosition = position;
        ParseFuncUtils.MoveWhile(self._condition, input, ref satisfiedPosition);
        var takeRange = new InputRange(position, satisfiedPosition - position);
        return ParseResult<TToken, InputRange, Unit>.Ok(satisfiedPosition, takeRange);
    }
}
