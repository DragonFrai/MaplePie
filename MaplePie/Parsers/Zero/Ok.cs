using MaplePie.Parser;

namespace MaplePie.Parsers.Zero;


public readonly struct OkParser<TToken, TOutput, TError>(TOutput output) : IParser<OkParser<TToken, TOutput, TError>, TToken, TOutput, TError>
{
    private readonly TOutput _output = output;
    
    public static ParseResult<TToken, TOutput, TError> Parse(ref OkParser<TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        return ParseResult<TToken, TOutput, TError>.Ok(position, self._output);
    }
}
