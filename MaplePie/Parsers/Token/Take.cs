using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Like Parse.Consumed(Parse.Skip(count))
/// </remarks>
/// <param name="count"></param>
/// <typeparam name="TToken"></typeparam>
public struct TakeParser<TToken>(int count) : IParser<TakeParser<TToken>, TToken, InputRange, Unit>
{
    private int _count = count;

    public static ParseResult<TToken, InputRange, Unit> Parse(ref TakeParser<TToken> self,
        ReadOnlySpan<TToken> input, int position)
    {
        var inputLen = input.Length - position;
        var takeCount = self._count;

        if (inputLen < takeCount)
        {
            return ParseResult<TToken, InputRange, Unit>.Miss(position, Unit.Instance);
        }

        var takeRange = new InputRange(position, takeCount);
        return ParseResult<TToken, InputRange, Unit>.Ok(position + takeCount, takeRange);
    }
}
