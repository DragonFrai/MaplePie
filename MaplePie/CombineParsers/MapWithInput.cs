using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.CombineParsers;

public struct MapWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>
    : IParser<MapWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
    where TParser : IParser<TParser, TToken, TOutput1, TError>
{
    private TParser _parser;
    private SpanArgFunc<TToken, TOutput1, TOutput2> _mapper;

    public MapWithInputParser(TParser parser, SpanArgFunc<TToken, TOutput1, TOutput2> mapper)
    {
        _parser = parser;
        _mapper = mapper;
    }

    public static
        ParseResult<TToken, TOutput2, TError> 
        Parse
        (
            ref MapWithInputParser<TParser, TToken, TOutput1, TOutput2, TError> self,
            ReadOnlySpan<TToken> input,
            int range
        )
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok =>
                ParseResult<TToken, TOutput2, TError>.Ok(
                    result.Position, 
                    self._mapper(input, result.Output)
                ),
            ParseResultKind.Miss => ParseResult<TToken, TOutput2, TError>.Miss(result.Position, result.Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput2, TError>.Fail(result.Position, result.Error),
            _ => throw new UnreachableException()
        };
    }
}