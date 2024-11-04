using MaplePie.Parser;

namespace MaplePie.ZeroParsers;


public readonly struct MissParser<TToken, TOutput, TError>(TError error) : IParser<MissParser<TToken, TOutput, TError>, TToken, TOutput, TError>
{
    private readonly TError _error = error;
    
    public static ParseResult<TToken, TOutput, TError> Parse(ref MissParser<TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        return ParseResult<TToken, TOutput, TError>.Miss(position, self._error);
    }
}
