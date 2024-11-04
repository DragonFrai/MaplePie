using System.Diagnostics;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Branch;


public struct Alt2Parser<TParser1, TParser2, TToken, TOutput, TError>(
    TParser1 parser1,
    TParser2 parser2,
    ValueSource<TError> noAlternativeError)
    : IParser<Alt2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser1 : IParser<TParser1, TToken, TOutput, TError>
    where TParser2 : IParser<TParser2, TToken, TOutput, TError>
{
    private TParser1 _parser1 = parser1;
    private TParser2 _parser2 = parser2;
    private ValueSource<TError> _noAlternativeError = noAlternativeError;

    public static
        ParseResult<TToken, TOutput, TError> Parse(ref Alt2Parser<TParser1, TParser2, TToken, TOutput, TError> self,
            ReadOnlySpan<TToken> input,
            int position)
    {
        var result1 = TParser1.Parse(ref self._parser1, input, position);
        switch (result1.Kind)
        {
            case ParseResultKind.Ok: 
                return ParseResult<TToken, TOutput, TError>.Ok(result1.Position, result1.Output);
            case ParseResultKind.Fail:
                return 
                    ParseResult<TToken, TOutput, TError>.Fail
                    (
                        result1.Position, 
                        result1.Error
                    );
            case ParseResultKind.Miss: break;
            default: throw new UnreachableException();
        }

        var result2 = TParser2.Parse(ref self._parser2, input, position);
        switch (result2.Kind)
        {
            case ParseResultKind.Ok: 
                return ParseResult<TToken, TOutput, TError>.Ok(result2.Position, result2.Output);
            case ParseResultKind.Fail:
                return 
                    ParseResult<TToken, TOutput, TError>.Fail
                    (
                        result2.Position, 
                        result2.Error
                    );
            case ParseResultKind.Miss: break;
            default: throw new UnreachableException();
        }

        return ParseResult<TToken, TOutput, TError>.Miss(position, self._noAlternativeError.Get());
    }
}

