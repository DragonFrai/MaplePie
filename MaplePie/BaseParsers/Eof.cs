using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public struct NotEofError {}

public struct EofParser<TToken> : IParser<EofParser<TToken>, TToken, Unit, NotEofError>
{
    public EofParser()
    {}

    public static ParseResult<TToken, Unit, NotEofError> Parse(ref EofParser<TToken> self, ReadOnlySpan<TToken> input, InputRange range)
    {
        if (range.Length != 0)
        {
            return ParseResult<TToken, Unit, NotEofError>.Miss(range, new NotEofError());
        }
        return ParseResult<TToken, Unit, NotEofError>.Ok(range, Unit.Instance);
    }
}
