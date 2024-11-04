using MaplePie.Parser;

namespace MaplePie.Parsers.Branch;

public struct IfElseParser<TParser1, TParser2, TToken, TOutput, TError>(bool condition, TParser1 parserIf, TParser2 parserElse)
    : IParser<IfElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser1 : IParser<TParser1, TToken, TOutput, TError>
    where TParser2 : IParser<TParser2, TToken, TOutput, TError>
{
    private readonly bool _condition = condition;
    private TParser1 _parserIf = parserIf;
    private TParser2 _parserElse = parserElse;
    
    public static ParseResult<TToken, TOutput, TError> Parse(ref IfElseParser<TParser1, TParser2, TToken, TOutput, TError> self, ReadOnlySpan<TToken> input, int position)
    {
        return
            self._condition
                ? TParser1.Parse(ref self._parserIf, input, position)
                : TParser2.Parse(ref self._parserElse, input, position);
    }
}