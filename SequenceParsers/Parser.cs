using System.Buffers;

namespace SequenceParsers;


/// <summary>
/// SequenceInput data stateful parser.
/// </summary>
/// <typeparam name="TToken"></typeparam>
/// <typeparam name="TOutput"></typeparam>
/// <typeparam name="TError"></typeparam>
public interface IParser<TToken, TOutput, TError>
{
    public ParseResult<TToken, TOutput, TError> Parse(SequenceInput<TToken> input);
    public void Reset();
}

public class ParserIsCompletedException : Exception
{
    public ParserIsCompletedException(): base() {}
    public ParserIsCompletedException(string? message): base(message) {}
    public ParserIsCompletedException(string? message, Exception? innerException): base(message, innerException) {}

    static void Throw() => throw new ParserIsCompletedException();
    static void Throw(string? message) => throw new ParserIsCompletedException(message);
    static void Throw(string? message, Exception? innerException) => 
        throw new ParserIsCompletedException(message, innerException);

}
