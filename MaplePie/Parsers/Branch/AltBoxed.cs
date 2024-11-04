using System.Diagnostics;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Branch;


public struct AltBoxedParser<TToken, TOutput, TError>(
    BoxedParser<TToken, TOutput, TError>[] parsers,
    ValueSource<TError> altNotChosenErrorSource)
    : IParser<AltBoxedParser<TToken, TOutput, TError>, TToken, TOutput, TError>
{
    private readonly BoxedParser<TToken, TOutput, TError>[] _parsers = parsers;
    private ValueSource<TError> _altNotChosenErrorSource = altNotChosenErrorSource;

    public static ParseResult<TToken, TOutput, TError> Parse(ref AltBoxedParser<TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        var current = 0;
        var length = self._parsers.Length;
        while (current < length)
        {
            var parser = self._parsers[current];
            var result = parser.Parse(input, position);

            switch (result.Kind)
            {
                case ParseResultKind.Ok:
                    return result;
                case ParseResultKind.Fail:
                    return result;
                case ParseResultKind.Miss:
                    current += 1;
                    continue;
                default: throw new UnreachableException();
            }
        }
        
        return ParseResult<TToken, TOutput, TError>.Miss(position, self._altNotChosenErrorSource.Get());
    }
}
