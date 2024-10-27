using SequenceParsers.Utils;

namespace SequenceParsers.BaseParsers;


public class EofParser<TToken> : IParser<TToken, Unit, Unit>
{
    public EofParser()
    {}

    public ParseResult<TToken, Unit, Unit> Parse(SequenceInput<TToken> input)
    {
        if (!(input.IsEmpty && input.IsCompleted))
        {
            return ParseResult<TToken, Unit, Unit>.Miss(input.Start, new Unit());
        }
        return ParseResult<TToken, Unit, Unit>.Ok(input.Start, new Unit());
    }

    public void Reset()
    { }
}