using System.Diagnostics;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Combine;


public struct OptionalParser<TParser, TOptionProxy, TToken, TOutput, TOptionOutput, TError>(TParser parser)
    : IParser<OptionalParser<TParser, TOptionProxy, TToken, TOutput, TOptionOutput, TError>, TToken, TOptionOutput, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>
    where TOptionProxy : IOption<TOptionOutput, TOutput>
{
    private TParser _parser = parser;

    public static
        ParseResult<TToken, TOptionOutput, TError> Parse(
            ref OptionalParser<TParser, TOptionProxy, TToken, TOutput, TOptionOutput, TError> self,
            ReadOnlySpan<TToken> input,
            int range)
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok => 
                ParseResult<TToken, TOptionOutput, TError>.Ok(result.Position, TOptionProxy.Some(result.Output)),
            ParseResultKind.Miss => 
                ParseResult<TToken, TOptionOutput, TError>.Ok(result.Position, TOptionProxy.None()),
            ParseResultKind.Fail => 
                ParseResult<TToken, TOptionOutput, TError>.Fail(result.Position, result.Error),
            _ => throw new UnreachableException()
        };
    }
}
