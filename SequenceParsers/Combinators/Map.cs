using System.Buffers;

namespace SequenceParsers.Combinators;


public class MapParser<TToken, TOutput1, TOutput2, TError> : IParser<TToken, TOutput2, TError>
{
    private IParser<TToken, TOutput1, TError> _parser;
    private Func<TOutput1, TOutput2> _mapper;

    public MapParser(IParser<TToken, TOutput1, TError> parser, Func<TOutput1, TOutput2> mapper)
    {
        _parser = parser;
        _mapper = mapper;
    }

    public ParseResult<TToken, TOutput2, TError> Parse(SequenceInput<TToken> input)
    {
        var result = _parser.Parse(input);
        return result.Map(_mapper);
    }

    public void Reset()
    {
        _parser.Reset();
    }
}
