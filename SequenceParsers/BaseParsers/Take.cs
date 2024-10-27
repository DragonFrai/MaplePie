using System.Buffers;

namespace SequenceParsers.BaseParsers;


public readonly struct TakeError(int expected, int actual)
{
    public int Expected => expected;
    public int Actual => actual;
}

public class TakeParser<TToken> : IParser<TToken, TToken[], TakeError>
{
    private int _count;

    public TakeParser(int count)
    {
        _count = count;
    }

    public ParseResult<TToken, TToken[], TakeError> Parse(SequenceInput<TToken> input)
    {
        var inputSeq = input.AsSequence;
        var inputLen = inputSeq.Length;
        
        if (inputLen < _count)
        {
            if (input.IsCompleted)
            {
                // TODO: use long
                var actual = (int)inputLen;
                return ParseResult<TToken, TToken[], TakeError>.Miss(inputSeq.Start, new TakeError(_count, actual));
            }
            else
            {
                return ParseResult<TToken, TToken[], TakeError>.Need(inputSeq.Start, Needed.Known(_count));
            }
        }

        var buffer = new TToken[_count];
        inputSeq.Slice(0, _count).CopyTo(buffer.AsSpan());

        return ParseResult<TToken, TToken[], TakeError>.Ok(inputSeq.GetPosition(_count), buffer);
    }

    public void Reset()
    { }
}
