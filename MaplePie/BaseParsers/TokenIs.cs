using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.BaseParsers;


public struct TokenIsParser<TToken>
    : IParser<TokenIsParser<TToken>, TToken, TToken, ParseError>
{
    private Func<TToken, bool> _condition;

    public TokenIsParser(Func<TToken, bool> condition)
    {
        _condition = condition;
    }

    public static
        ParseResult<TToken, TToken, ParseError> 
        Parse
        (
            ref TokenIsParser<TToken> self,
            ReadOnlySpan<TToken> input,
            int position
        )
    {
        if (position == input.Length)
        {
            return ParseResult<TToken, TToken, ParseError>.Miss(position, ParseError.TokenIs);
        }

        var token = input[position];
        if (self._condition(token))
        {
            return ParseResult<TToken, TToken, ParseError>.Ok(position + 1, token);
        }
        
        return ParseResult<TToken, TToken, ParseError>.Miss(position, ParseError.TokenIs);
    }
}