using System.Buffers;
using SequenceParsers.Utils;

namespace SequenceParsers.Combinators;


public class SetErrorParser<TToken, TOutput, TError1, TError2> : IParser<TToken, TOutput, TError2>
{
    private IParser<TToken, TOutput, TError1> _parser;
    private ValueSource<TError2> _errorSource;

    public SetErrorParser(IParser<TToken, TOutput, TError1> parser, ValueSource<TError2> errorSource)
    {
        _parser = parser;
        _errorSource = errorSource;
    }

    public ParseResult<TToken, TOutput, TError2> Parse(SequenceInput<TToken> input)
    {
        return _parser.Parse(input).MapError(_ => _errorSource.Get());
    }

    public void Reset()
    {
        _parser.Reset();
    }
}
