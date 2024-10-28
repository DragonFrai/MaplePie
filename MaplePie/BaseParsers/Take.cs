using System.Buffers;
using MaplePie.Utils;

namespace MaplePie.BaseParsers;

public readonly struct TakeError(int expected, int actual)
{
    public int Expected => expected;
    public int Actual => actual;
}

public struct TakeParser<TToken> : IParser<TakeParser<TToken>, TToken, InputRange, TakeError>
{
    private int _count;

    public TakeParser(int count)
    {
        _count = count;
    }

    public static ParseResult<TToken, InputRange, TakeError> Parse(ref TakeParser<TToken> self, ReadOnlySpan<TToken> input, InputRange range)
    {
        var inputLen = range.Length;

        if (inputLen < self._count)
        {
            return ParseResult<TToken, InputRange, TakeError>.Miss(range, new TakeError(self._count, inputLen));
        }

        range.Split(self._count, out var result, out var remains);

        return ParseResult<TToken, InputRange, TakeError>.Ok(remains, result);
    }
}
