using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.Parsers.Combine;

/// <summary>
/// Map passed parser error to passed error
/// </summary>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TParser"></typeparam>
/// <typeparam name="TError1"></typeparam>
/// <typeparam name="TError2"></typeparam>
public struct SetErrorParser<TParser, TToken, TOutput, TError1, TError2>(TParser parser, TError2 error)
    : IParser<SetErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
    where TParser : IParser<TParser, TToken, TOutput, TError1>
{
    private TParser _parser = parser;
    private readonly TError2 _error = error;


    public static ParseResult<TToken, TOutput, TError2> Parse(ref SetErrorParser<TParser, TToken, TOutput, TError1, TError2> self, ReadOnlySpan<TToken> input, int position)
    {
        var result = TParser.Parse(ref self._parser, input, position);

        return result.Kind switch
        {
            ParseResultKind.Ok => ParseResult<TToken, TOutput, TError2>.Ok(result.Position, result.Output),
            ParseResultKind.Miss => ParseResult<TToken, TOutput, TError2>.Miss(result.Position, self._error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput, TError2>.Fail(result.Position, self._error),
            _ => throw new UnreachableException()
        };
    }
}
