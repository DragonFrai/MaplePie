using System.Diagnostics;

namespace MaplePie.Parser;


/// <summary>
/// Struct-based parser interface.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TError"></typeparam>
public interface IParser<TSelf, TToken, TOutput, TError> where TSelf: IParser<TSelf, TToken, TOutput, TError>
{
    // TODO: Compose (ReadOnlySpan<TToken> input, InputRange range) into one structure.
    public static abstract 
        ParseResult<TToken, TOutput, TError> 
        Parse(
            ref TSelf self, 
            ReadOnlySpan<TToken> input,
            int position
        );
}

/// <summary>
/// Struct-based parser implementation wrapper.
/// Designed for more easy method invocations (Without specify all generics in extensions methods).
/// Any struct-based parser should be wrapped on it before getting into the user code.
/// </summary>
/// <param name="parser"></param>
/// <typeparam name="TParser"></typeparam>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TError"></typeparam>
public struct Parser<TParser, TToken, TOutput, TError>(TParser parser)
    where TParser : IParser<TParser, TToken, TOutput, TError>
{
    public TParser Inner = parser;
}

public static class FundamentalParserExtensions
{
    public static ParseResult<TToken, TOutput, TError> Parse<TParser, TToken, TOutput, TError>(
        this ref Parser<TParser, TToken, TOutput, TError> parser, ReadOnlySpan<TToken> input, int position) where TParser: IParser<TParser, TToken, TOutput, TError>
    {
        return TParser.Parse(ref parser.Inner, input, position);
    }

    public static ParseResult<TToken, TOutput, TError> Parse<TParser, TToken, TOutput, TError>(
        this ref Parser<TParser, TToken, TOutput, TError> parser, ReadOnlySpan<TToken> input) where TParser: IParser<TParser, TToken, TOutput, TError>
    {
        return TParser.Parse(ref parser.Inner, input, 0);
    }
}
