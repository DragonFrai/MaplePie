using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Branch;

public struct IfParser<TParser, TToken, TOutput, TError>(bool condition, TParser parser, ValueSource<TError> elseError)
    : IParser<IfParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>
{
    private readonly bool _condition = condition;
    private TParser _parser = parser;
    private ValueSource<TError> _elseError = elseError;

    public static ParseResult<TToken, TOutput, TError> Parse(ref IfParser<TParser, TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        return
            self._condition
                ? TParser.Parse(ref self._parser, input, position)
                : ParseResult<TToken, TOutput, TError>.Miss(position, self._elseError.Get());
    }
}
