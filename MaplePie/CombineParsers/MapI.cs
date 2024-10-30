using System.Diagnostics;
using MaplePie.Parser;

namespace MaplePie.CombineParsers;

public struct MapIParser<TParser, TToken, TOutput, TError>
    : IParser<MapIParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser : IParser<TParser, TToken, InputRange, TError>
{
    private TParser _parser;
    private SpanFunc<TToken, TOutput> _mapper;

    public MapIParser(TParser parser, SpanFunc<TToken, TOutput> mapper)
    {
        _parser = parser;
        _mapper = mapper;
    }

    public static
        ParseResult<TToken, TOutput, TError> Parse(ref MapIParser<TParser, TToken, TOutput, TError> self,
            ReadOnlySpan<TToken> input,
            int range)
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok =>
                ParseResult<TToken, TOutput, TError>.Ok(result.Position, self._mapper(input.Slice(result.Output.Position, result.Output.Length))),
            ParseResultKind.Miss => ParseResult<TToken, TOutput, TError>.Miss(result.Position, result.Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput, TError>.Fail(result.Position, result.Error),
            _ => throw new UnreachableException()
        };
    }
}
