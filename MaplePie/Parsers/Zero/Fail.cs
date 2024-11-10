using MaplePie.Parser;

namespace MaplePie.Parsers.Zero;


public readonly struct FailParser<TToken, TOutput, TError>(TError error) : IParser<FailParser<TToken, TOutput, TError>, TToken, TOutput, TError>
{
    private readonly TError _error = error;
    
    public static ParseResult<TToken, TOutput, TError> Parse(ref FailParser<TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        return ParseResult<TToken, TOutput, TError>.Fail(position, self._error);
    }
}
