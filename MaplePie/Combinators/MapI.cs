using System.Diagnostics;

namespace MaplePie.Combinators;

public struct MapIParser<TParser, TToken, TOutput, TError>
    : IParser<MapIParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser : struct, IParser<TParser, TToken, InputRange, TError>
{
    private TParser _parser;
    private SpanFunc<TToken, TOutput> _mapper;

    public MapIParser(TParser parser, SpanFunc<TToken, TOutput> mapper)
    {
        _parser = parser;
        _mapper = mapper;
    }

    public static 
        ParseResult<TToken, TOutput, TError> 
        Parse(
            ref MapIParser<TParser, TToken, TOutput, TError> self, 
            ReadOnlySpan<TToken> input, 
            InputRange range)
    {
        var result = TParser.Parse(ref self._parser, input, range);
        return result.Kind switch
        {
            ParseResultKind.Ok =>
                ParseResult<TToken, TOutput, TError>.Ok(result.RemainderRange, self._mapper(input.Slice(result.Output.CursorRange))),
            ParseResultKind.Miss => ParseResult<TToken, TOutput, TError>.Miss(result.RemainderRange, result.Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput, TError>.Fail(result.RemainderRange, result.Error),
            _ => throw new UnreachableException()
        };
    }
}