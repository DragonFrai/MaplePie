using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public enum TagErrorKind
{
    NotMatched,
    UnexpectedEof,
}

public readonly struct TagError<TToken>(ReadOnlyMemory<TToken> expectedTag, TagErrorKind kind)
{
    public ReadOnlyMemory<TToken> ExpectedTag => expectedTag;
    public TagErrorKind Kind => kind;
}

public struct TagParser<TToken> : IParser<TagParser<TToken>, TToken, InputRange, TagError<TToken>>
{
    // Expected tag sequence
    private readonly ReadOnlyMemory<TToken> _tag;

    public TagParser(ReadOnlyMemory<TToken> tag)
    {
        _tag = tag;
    }

    public static ParseResult<TToken, InputRange, TagError<TToken>> Parse(ref TagParser<TToken> self, ReadOnlySpan<TToken> input, InputRange range)
    {
        var tagLength = self._tag.Length;
        
        if (range.Length < tagLength)
        {
            return ParseResult<TToken, InputRange, TagError<TToken>>.Miss(range, new TagError<TToken>(self._tag, TagErrorKind.UnexpectedEof));
        }

        range.Split(tagLength, out var tagRange, out var remainderRange);

        var segment = input.Slice(tagRange.CursorRange);
        var isEqual = segment.SequenceEqual(self._tag.Span);

        if (!isEqual)
        {
            return ParseResult<TToken, InputRange, TagError<TToken>>.Miss(range, new TagError<TToken>(self._tag, TagErrorKind.NotMatched));
        }
        
        return ParseResult<TToken, InputRange, TagError<TToken>>.Ok(remainderRange, tagRange);
    }
}
