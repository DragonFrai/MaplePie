using MaplePie.Errors;
using MaplePie.Parser;

namespace MaplePie.BaseParsers;

public struct TakeParser<TToken> 
    : IParser<TakeParser<TToken>, TToken, InputRange, ParseError>
{
    private int _count;

    public TakeParser(int count)
    {
        _count = count;
    }

    public static ParseResult<TToken, InputRange, ParseError> Parse(ref TakeParser<TToken> self,
        ReadOnlySpan<TToken> input, int position)
    {
        var inputLen = input.Length - position;
        var takeCount = self._count;

        if (inputLen < takeCount)
        {
            return ParseResult<TToken, InputRange, ParseError>.Miss(position, ParseError.Take);
        }

        var takeRange = new InputRange(position, takeCount);
        return ParseResult<TToken, InputRange, ParseError>.Ok(position + takeCount, takeRange);
    }
}
