using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.Parsers.Combine;


public struct MapErrorParser<TParser, TToken, TOutput, TError1, TError2>(
    TParser parser,
    Func<TError1, TError2> errorMapper)
    : IParser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
    where TParser : IParser<TParser, TToken, TOutput, TError1>
{
    private TParser _parser = parser;
    private readonly Func<TError1, TError2> _errorMapper = errorMapper;

    public static
        ParseResult<TToken, TOutput, TError2> Parse(ref MapErrorParser<TParser, TToken, TOutput, TError1, TError2> self,
            ReadOnlySpan<TToken> input,
            int range)
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok => ParseResult<TToken, TOutput, TError2>.Ok(result.Position, result.Output),
            ParseResultKind.Miss => ParseResult<TToken, TOutput, TError2>.Miss(result.Position, self._errorMapper(result.Error)),
            ParseResultKind.Fail => ParseResult<TToken, TOutput, TError2>.Fail(result.Position, self._errorMapper(result.Error)),
            _ => throw new UnreachableException()
        };
    }
}
