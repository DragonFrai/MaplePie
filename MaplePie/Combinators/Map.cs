using System.Diagnostics;

namespace MaplePie.Combinators;


public struct MapParser<TParser, TToken, TOutput1, TOutput2, TError>
    : IParser<MapParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
    where TParser : struct, IParser<TParser, TToken, TOutput1, TError>
{
    private TParser _parser;
    private Func<TOutput1, TOutput2> _mapper;

    public MapParser(TParser parser, Func<TOutput1, TOutput2> mapper)
    {
        _parser = parser;
        _mapper = mapper;
    }

    public static 
        ParseResult<TToken, TOutput2, TError> 
        Parse(
            ref MapParser<TParser, TToken, TOutput1, TOutput2, TError> self, 
            ReadOnlySpan<TToken> input, 
            InputRange range)
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok => 
                ParseResult<TToken, TOutput2, TError>.Ok(result.RemainderRange, self._mapper(result.Output)),
            ParseResultKind.Miss => ParseResult<TToken, TOutput2, TError>.Miss(result.RemainderRange, result.Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput2, TError>.Fail(result.RemainderRange, result.Error),
            _ => throw new UnreachableException()
        };
    }
}