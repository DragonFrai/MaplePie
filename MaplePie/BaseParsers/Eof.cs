using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public struct EofParser<TToken> : IParser<EofParser<TToken>, TToken, Unit, ParseError>
{
    public EofParser()
    {}

    public static 
        ParseResult<TToken, Unit, ParseError> 
        Parse(
            ref EofParser<TToken> self, 
            ReadOnlySpan<TToken> input,
            int position
        )
    {
        if (position != input.Length)
        {
            return ParseResult<TToken, Unit, ParseError>.Miss(position, ParseError.Eof);
        }
        return ParseResult<TToken, Unit, ParseError>.Ok(position, Unit.Instance);
    }
}
