using System.Buffers;
using SequenceParsers.Utils;

namespace SequenceParsers.BaseParsers;


public readonly struct TakeWhileError(int expected, int actual)
{
    public int Expected => expected;
    public int Actual => actual;
}

public class TakeWhileParser<TToken> : IParser<TToken, TToken[], Never>
{
    private Func<TToken, bool> _condition;
    // private int _consumed;

    public TakeWhileParser(Func<TToken, bool> condition)
    {
        _condition = condition;
        // _consumed = 0;
    }

    public ParseResult<TToken, TToken[], Never> Parse(SequenceInput<TToken> input)
    {
        var inputSeq = input.AsSequence;

        var consumed = 0;
        var endTokenReached = false;
        foreach (var segment in inputSeq)
        {
            foreach (var token in segment.Span)
            {
                if (_condition.Invoke(token))
                {
                    consumed += 1;
                }
                else
                {
                    endTokenReached = true;
                    goto ret;
                }
            }
        }
        ret:

        if (!(endTokenReached || input.IsCompleted))
        {
            return ParseResult<TToken, TToken[], Never>.Need(inputSeq.Start, Needed.Unknown);
        }

        var buffer = new TToken[consumed];
        inputSeq.Slice(0, consumed).CopyTo(buffer.AsSpan());
        return ParseResult<TToken, TToken[], Never>.Ok(inputSeq.GetPosition(consumed), buffer);
    }

    public void Reset()
    {
        // _consumed = 0;
    }
}