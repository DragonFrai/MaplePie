using System.Diagnostics;

namespace MaplePie.Combinators;


public struct MapErrorParser<TParser, TToken, TOutput, TError1, TError2> 
    : IParser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
    where TParser: struct, IParser<TParser, TToken, TOutput, TError1>
{
    private TParser _parser;
    private Func<TError1, TError2> _errorMapper;

    public MapErrorParser(TParser parser, Func<TError1, TError2> errorMapper)
    {
        _parser = parser;
        _errorMapper = errorMapper;
    }
    
    public static 
        ParseResult<TToken, TOutput, TError2> 
        Parse(
            ref MapErrorParser<TParser, TToken, TOutput, TError1, TError2> self, 
            ReadOnlySpan<TToken> input, 
            InputRange range)
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok => ParseResult<TToken, TOutput, TError2>.Ok(result.RemainderRange, result.Output),
            ParseResultKind.Miss => ParseResult<TToken, TOutput, TError2>.Miss(result.RemainderRange, self._errorMapper(result.Error)),
            ParseResultKind.Fail => ParseResult<TToken, TOutput, TError2>.Fail(result.RemainderRange, self._errorMapper(result.Error)),
            _ => throw new UnreachableException()
        };
    }
}
