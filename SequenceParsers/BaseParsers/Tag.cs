namespace SequenceParsers.BaseParsers;


public readonly struct TagError<TToken>(TToken[] expectedTag)
{
    public TToken[] ExpectedTag => expectedTag;
}

public class TagParser<TToken> : IParser<TToken, TToken[], TagError<TToken>>
{
    // Expected tag sequence
    private readonly TToken[] _tag;

    public TagParser(TToken[] tag)
    {
        _tag = tag;
    }

    public ParseResult<TToken, TToken[], TagError<TToken>> Parse(SequenceInput<TToken> input)
    {
        var inputSeq = input.AsSequence;
        
        if (inputSeq.Length < _tag.Length)
        {
            return ParseResult<TToken, TToken[], TagError<TToken>>.Need(inputSeq.Start, Needed.Known(_tag.Length));
        }

        var offset = 0;
        var inputTag = inputSeq.Slice(0, _tag.Length);
        while (!inputTag.IsEmpty)
        {
            var inputSpan = inputTag.FirstSpan;
            var len = inputSpan.Length;
            var tagSpan = _tag.AsSpan(offset, len);
            var segmentEqual = inputSpan.SequenceEqual(tagSpan);

            if (!segmentEqual)
            {
                return ParseResult<TToken, TToken[], TagError<TToken>>.Miss(inputTag.Start, new TagError<TToken>(_tag));
            }

            offset += len;
            inputTag = inputTag.Slice(len);
        }
        
        return ParseResult<TToken, TToken[], TagError<TToken>>.Ok(inputTag.Start, _tag);
    }

    public void Reset()
    {
        // Not accumulate states
    }
}


public class LazyTagParser<TToken> : IParser<TToken, TToken[], TagError<TToken>>
{
    // Expected tag sequence
    private readonly TToken[] _tag;
    
    // Current parsing state. Contains already checked elements count.
    private int _offset;

    public LazyTagParser(TToken[] tag)
    {
        _tag = tag;
        _offset = 0;
    }

    public ParseResult<TToken, TToken[], TagError<TToken>> Parse(SequenceInput<TToken> input)
    {
        var inputSeq = input.AsSequence;
        
        var inputTag = inputSeq.Slice(0, Math.Min(_tag.Length - _offset, inputSeq.Length));
        while (!inputTag.IsEmpty)
        {
            var inputSpan = inputTag.FirstSpan;
            var len = inputSpan.Length;
            var tagSpan = _tag.AsSpan(_offset, len);
            var segmentEqual = inputSpan.SequenceEqual(tagSpan);

            if (!segmentEqual)
            {
                return ParseResult<TToken, TToken[], TagError<TToken>>.Miss(inputTag.Start, new TagError<TToken>(_tag));
            }

            _offset += len;
            inputTag = inputTag.Slice(len);
        }
        
        if (_offset != _tag.Length)
        {
            return ParseResult<TToken, TToken[], TagError<TToken>>.Need(inputTag.Start, Needed.Known(_tag.Length));
        }

        return ParseResult<TToken, TToken[], TagError<TToken>>.Ok(inputTag.Start, _tag);
    }

    public void Reset()
    {
        _offset = 0;
    }
}
