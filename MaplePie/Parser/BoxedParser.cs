namespace MaplePie.Parser;


/// <summary>
/// Interface-based parser interface.
/// When possible, use BoxedParser instead IBoxedParser, because BoxedParser class is IParser.
/// </summary>
public interface IBoxedParser<TToken, TOutput, TError>
{
    public ParseResult<TToken, TOutput, TError> Parse(ReadOnlySpan<TToken> input, int position);
}

public abstract class BoxedParser<TToken, TOutput, TError>: 
    IBoxedParser<TToken, TOutput, TError>,
    IParser<BoxedParser<TToken, TOutput, TError>, TToken, TOutput, TError>
{
    public abstract ParseResult<TToken, TOutput, TError> Parse(ReadOnlySpan<TToken> input, int position);
    
    public static
        ParseResult<TToken, TOutput, TError> Parse
        (ref BoxedParser<TToken, TOutput, TError> self,
            ReadOnlySpan<TToken> input,
            int range)
    {
        return self.Parse(input, range);
    }
}

public class BoxedParserImpl<TParser, TToken, TOutput, TError>(TParser parser): 
    BoxedParser<TToken, TOutput, TError>
    where TParser: IParser<TParser, TToken, TOutput, TError>
{
    public override ParseResult<TToken, TOutput, TError> Parse(ReadOnlySpan<TToken> input, int position)
    {
        return TParser.Parse(ref parser, input, position);
    }
}

public static class FundamentalBoxedParserExtensions
{
    public static 
        BoxedParser<TToken, TOutput, TError> 
        ToBoxed<TParser, TToken, TOutput, TError>
        (
            this Parser<TParser, TToken, TOutput, TError> parser
        )
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new BoxedParserImpl<TParser, TToken, TOutput, TError>(parser.Inner);
    }
    
    public static 
        Parser<BoxedParser<TToken, TOutput, TError>, TToken, TOutput, TError> 
        ToParser<TToken, TOutput, TError>
        (
            this BoxedParser<TToken, TOutput, TError> boxedParser
        )
    {
        return new Parser<BoxedParser<TToken, TOutput, TError>, TToken, TOutput, TError>(boxedParser);
    }
    
    public static 
        Parser<BoxedParser<TToken, TOutVal, TError>, TToken, TOutVal, TError> 
        ToBoxedParser<TParser, TToken, TOutVal, TError>
        (
            this Parser<TParser, TToken, TOutVal, TError> parser
        )
        where TParser : IParser<TParser, TToken, TOutVal, TError>
    {
        return parser.ToBoxed().ToParser();
    }
}
