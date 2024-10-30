using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public struct TagParser<TToken> : IParser<TagParser<TToken>, TToken, InputRange, ParseError>
{
    // Expected tag sequence
    private readonly ReadOnlyMemory<TToken> _tag;

    public TagParser(ReadOnlyMemory<TToken> tag)
    {
        _tag = tag;
    }

    public static 
        ParseResult<TToken, InputRange, ParseError> Parse
        (
            ref TagParser<TToken> self,
            ReadOnlySpan<TToken> input, 
            int position
        )
    {
        var tagLength = self._tag.Length;
        var remLength = input.Length - position;
        
        if (remLength < tagLength)
        {
            return ParseResult<TToken, InputRange, ParseError>.Miss(position, ParseError.Tag);
        }

        var segment = input.Slice(position, tagLength);

        var isEqual = segment.SequenceEqual(self._tag.Span);
        if (!isEqual)
        {
            return ParseResult<TToken, InputRange, ParseError>.Miss(position, ParseError.Tag);
        }

        var tagRange = new InputRange(position, tagLength);
        return ParseResult<TToken, InputRange, ParseError>.Ok(position + tagLength, tagRange);
    }
}
