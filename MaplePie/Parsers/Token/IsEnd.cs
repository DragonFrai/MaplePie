using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Token;


public struct IsEndParser<TToken> : IParser<IsEndParser<TToken>, TToken, Unit, Unit>
{
    public IsEndParser()
    {}

    public static 
        ParseResult<TToken, Unit, Unit> 
        Parse(
            ref IsEndParser<TToken> self, 
            ReadOnlySpan<TToken> input,
            int position
        )
    {
        return 
            position != input.Length 
            ? ParseResult<TToken, Unit, Unit>.Miss(position, Unit.Instance) 
            : ParseResult<TToken, Unit, Unit>.Ok(position, Unit.Instance);
    }
}
