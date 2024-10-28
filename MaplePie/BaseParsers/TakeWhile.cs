using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public struct TakeWhileParser<TToken> 
    : IParser<TakeWhileParser<TToken>, TToken, InputRange, Never>
{
    private Func<TToken, bool> _condition;

    public TakeWhileParser(Func<TToken, bool> condition)
    {
        _condition = condition;
    }

    public static 
        ParseResult<TToken, InputRange, Never> 
        Parse(
            ref TakeWhileParser<TToken> self, 
            ReadOnlySpan<TToken> input, 
            InputRange range)
    {
        var satisfied = 0;
        for (var i = 0; i < range.Length; i++)
        {
            var token = input[range.Position + i];
            if (!self._condition(token))
            {
                satisfied = i;
                break;
            }
        }
        
        range.Split(satisfied, out var result, out var reminder);

        return ParseResult<TToken, InputRange, Never>.Ok(reminder, result);
    }
}