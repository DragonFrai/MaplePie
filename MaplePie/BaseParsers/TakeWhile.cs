using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public struct TakeWhileParser<TToken> 
    : IParser<TakeWhileParser<TToken>, TToken, InputRange, ParseError>
{
    private Func<TToken, bool> _condition;

    public TakeWhileParser(Func<TToken, bool> condition)
    {
        _condition = condition;
    }

    public static
        ParseResult<TToken, InputRange, ParseError> Parse(ref TakeWhileParser<TToken> self,
            ReadOnlySpan<TToken> input,
            int position)
    {
        var satisfiedPosition = -1;
        for (var i = position; i < input.Length; i++)
        {
            var token = input[i];
            if (!self._condition(token))
            {
                satisfiedPosition = i;
                break;
            }
        }

        var takeRange = new InputRange(position, satisfiedPosition - position);
        return ParseResult<TToken, InputRange, ParseError>.Ok(satisfiedPosition, takeRange);
    }
}