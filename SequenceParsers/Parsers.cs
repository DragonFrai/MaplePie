using System.Buffers;
using SequenceParsers.BaseParsers;
using SequenceParsers.Branches;
using SequenceParsers.Combinators;
using SequenceParsers.Utils;

namespace SequenceParsers;

public static class Parsers
{
    public static TagParser<TToken> Tag<TToken>(TToken[] tag)
    {
        return new TagParser<TToken>(tag);
    }

    public static LazyTagParser<TToken> LazyTag<TToken>(TToken[] tag)
    {
        return new LazyTagParser<TToken>(tag);
    }

    public static TakeParser<TToken> Take<TToken>(int count)
    {
        return new TakeParser<TToken>(count);
    }

    public static TakeWhileParser<TToken> TakeWhile<TToken>(Func<TToken, bool> condition)
    {
        return new TakeWhileParser<TToken>(condition);
    }

    public static AnyParser<TToken, TOutput, TError> Any<TToken, TOutput, TError>(
        IParser<TToken, TOutput, TError>[] parsers, TError error)
    {
        return new AnyParser<TToken, TOutput, TError>(parsers, ValueSource<TError>.Value(error));
    }

    public static AnyParser<TToken, TOutput, TError> AnyWith<TToken, TOutput, TError>(
        IParser<TToken, TOutput, TError>[] parsers, Func<TError> errorFunc)
    {
        return new AnyParser<TToken, TOutput, TError>(parsers, ValueSource<TError>.Func(errorFunc));
    }

    public static EofParser<TToken> Eof<TToken>()
    {
        return new EofParser<TToken>();
    }
}

public static class ParserExtensions
{
    public static ParseResult<TToken, TOutput, TError> ParseAndReset<TToken, TOutput, TError>(
        this IParser<TToken, TOutput, TError> parser, SequenceInput<TToken> input)
    {
        var result = parser.Parse(input);
        if (result.Kind != ParseResultKind.Need)
        {
            parser.Reset();
        }
        return result;
    }

    public static SetErrorParser<TToken, TOutput, TError1, TError2> SetError<TToken, TOutput, TError1, TError2>(
        this IParser<TToken, TOutput, TError1> parser, TError2 error)
    {
        return new SetErrorParser<TToken, TOutput, TError1, TError2>(parser, ValueSource<TError2>.Value(error));
    }

    public static SetErrorParser<TToken, TOutput, TError1, TError2> SetErrorWith<TToken, TOutput, TError1, TError2>(
        this IParser<TToken, TOutput, TError1> parser, Func<TError2> errorFunc)
    {
        return new SetErrorParser<TToken, TOutput, TError1, TError2>(parser, ValueSource<TError2>.Func(errorFunc));
    }

    public static SetErrorParser<TToken, TOutput, TError, Unit> IgnoreError<TToken, TOutput, TError>(
        this IParser<TToken, TOutput, TError> parser)
    {
        return new SetErrorParser<TToken, TOutput, TError, Unit>(parser, ValueSource<Unit>.Value(new Unit()));
    }

    public static MapParser<TToken, TOutput1, TOutput2, TError> Map<TToken, TOutput1, TOutput2, TError>(
        this IParser<TToken, TOutput1, TError> parser, Func<TOutput1, TOutput2> mapper)
    {
        return new MapParser<TToken, TOutput1, TOutput2, TError>(parser, mapper);
    }

    public static BindParser<TToken, TOutput1, TOutput2, TError> Bind<TToken, TOutput1, TOutput2, TError>(
        this IParser<TToken, TOutput1, TError> parser, Func<TOutput1, IParser<TToken, TOutput2, TError>> binder)
    {
        return new BindParser<TToken, TOutput1, TOutput2, TError>(parser, binder);
    }
}
