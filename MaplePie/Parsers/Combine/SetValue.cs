using System.Diagnostics;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Combine;

/// <summary>
/// Map passed parser output to passed value
/// </summary>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput1"></typeparam>
/// <typeparam name="TParser"></typeparam>
/// <typeparam name="TOutput2"></typeparam>
/// <typeparam name="TError"></typeparam>
public struct SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>(TParser parser, TOutput2 value)
    : IParser<SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
    where TParser : IParser<TParser, TToken, TOutput1, TError>
{
    private TParser _parser = parser;
    private readonly TOutput2 _value = value;

    public static 
        ParseResult<TToken, TOutput2, TError> 
        Parse(
            ref SetValueParser<TParser, TToken, TOutput1, TOutput2, TError> self, 
            ReadOnlySpan<TToken> input, 
            int position)
    {
        var result = TParser.Parse(ref self._parser, input, position);

        return result.Kind switch
        {
            ParseResultKind.Ok => ParseResult<TToken, TOutput2, TError>.Ok(result.Position, self._value),
            ParseResultKind.Miss => ParseResult<TToken, TOutput2, TError>.Miss(result.Position, result.Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput2, TError>.Fail(result.Position, result.Error),
            _ => throw new UnreachableException()
        };
    }
}
