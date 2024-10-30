using System.Text;
using MaplePie.CombineParsers;
using MaplePie.Parser;

namespace MaplePie.Errors;


public class ContextError
{
    private Stack<string> _contexts;

    public ContextError(string context)
    {
        var contexts = new Stack<string>(4);
        contexts.Push(context);
        _contexts = contexts;
    }

    public ContextError AddContext(string context)
    {
        _contexts.Push(context);
        return this;
    }

    public string ToString(string separator)
    {
        var sb = new StringBuilder();
        sb.AppendJoin(separator, _contexts);
        return sb.ToString();
    }
    
    public override string ToString()
    {
        return ToString("=>");
    }

    public string Format<TToken>(ReadOnlySpan<TToken> input, InputRange range)
    {
        throw new NotImplementedException();
    }

    // public override bool Equals(object? other)
    // {
    //     if (other is ContextError otherContextError)
    //     {
    //         return Equals(otherContextError);
    //     }
    //     return false;
    // }
    //
    // public bool Equals(ContextError other)
    // {
    //     return Equals(_parent, other._parent) && _contextMessage == other._contextMessage;
    // }
    //
    // public override int GetHashCode()
    // {
    //     return HashCode.Combine(_parent, _contextMessage);
    // }
}

public static class ParserContextErrorExtensions
{
    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, TError, ContextError>, TToken, TOutput, ContextError>
        SetContext<TParser, TToken, TOutput, TError>
        (
            this Parser<TParser, TToken, TOutput, TError> parser,
            string context
        )
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return parser.MapError(_ => new ContextError(context));
    }
    
    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, ContextError, ContextError>, TToken, TOutput, ContextError>
        AddContext<TParser, TToken, TOutput>
        (
            this Parser<TParser, TToken, TOutput, ContextError> parser,
            string context
        )
        where TParser : IParser<TParser, TToken, TOutput, ContextError>
    {
        return parser.MapError(ce => ce.AddContext(context));
    }

    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, ParseError, ContextError>, TToken, TOutput, ContextError>
        ToContextual<TParser, TToken, TOutput>
        (
            this Parser<TParser, TToken, TOutput, ParseError> parser
        )
        where TParser : IParser<TParser, TToken, TOutput, ParseError>
    {
        return parser.MapError(pe => new ContextError(pe.ToString()));
    }
}
