using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.Parsers.Combine;


/// <summary>
/// Parser that returns InputRange consumed by passed parser.
/// </summary>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TParser"></typeparam>
/// <typeparam name="TError"></typeparam>
public struct ConsumedParser<TParser, TToken, TOutput, TError>(TParser parser)
    : IParser<ConsumedParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>
{
    private TParser _parser = parser;

    public static 
        ParseResult<TToken, InputRange, TError> 
        Parse(
            ref ConsumedParser<TParser, TToken, TOutput, TError> self, 
            ReadOnlySpan<TToken> input, 
            int position)
    {
        var result = TParser.Parse(ref self._parser, input, position);
        return result.Kind switch
        {
            ParseResultKind.Ok => 
                ParseResult<TToken, InputRange, TError>.Ok(
                    result.Position, new InputRange(position, result.Position - position)
                ),
            ParseResultKind.Miss => ParseResult<TToken, InputRange, TError>.Miss(result.Position, result.Error),
            ParseResultKind.Fail => ParseResult<TToken, InputRange, TError>.Fail(result.Position, result.Error),
            _ => throw new UnreachableException()
        };
    }
}
