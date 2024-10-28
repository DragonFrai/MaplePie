using System.Buffers;
using MaplePie.BaseParsers;
using MaplePie.Branches;
using MaplePie.Combinators;
using MaplePie.Utils;

namespace MaplePie;

public static class Parsers
{
    public static Parser<TagParser<TToken>, TToken, InputRange, TagError<TToken>> Tag<TToken>(TToken[] tag)
    {
        return new Parser<TagParser<TToken>, TToken, InputRange, TagError<TToken>>(
            new TagParser<TToken>(tag));
    }

    public static Parser<TakeParser<TToken>, TToken, InputRange, TakeError> Take<TToken>(int count)
    {
        return new Parser<TakeParser<TToken>, TToken, InputRange, TakeError>(
            new TakeParser<TToken>(count));
    }

    public static
        Parser<TakeWhileParser<TToken>, TToken, InputRange, Never>
        TakeWhile<TToken>(Func<TToken, bool> condition)
    {
        return new Parser<TakeWhileParser<TToken>, TToken, InputRange, Never>(
            new TakeWhileParser<TToken>(condition));
    }

    // public static AnyBoxedParser<TToken, TOutVal, TError> Any<TToken, TOutVal, TError>(
    //     IBoxedParser<TToken, TOutVal, TError>[] parsers, TError error)
    // {
    //     return new AnyBoxedParser<TToken, TOutVal, TError>(parsers, ValueSource<TError>.Value(error));
    // }
    //
    // public static AnyBoxedParser<TToken, TOutVal, TError> AnyWith<TToken, TOutVal, TError>(
    //     IBoxedParser<TToken, TOutVal, TError>[] parsers, Func<TError> errorFunc)
    // {
    //     return new AnyBoxedParser<TToken, TOutVal, TError>(parsers, ValueSource<TError>.Func(errorFunc));
    // }

    public static Parser<EofParser<TToken>, TToken, Unit, NotEofError> Eof<TToken>()
    {
        return new Parser<EofParser<TToken>, TToken, Unit, NotEofError>(new EofParser<TToken>());
    }
}

public delegate TOutput SpanFunc<TToken, TOutput>(ReadOnlySpan<TToken> span);

public static class ParserExtensions
{
    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
        MapError<TParser, TToken, TOutput, TError1, TError2>(
            this Parser<TParser, TToken, TOutput, TError1> parser, Func<TError1, TError2> errorMapper)
        where TParser : struct, IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>(
            new MapErrorParser<TParser, TToken, TOutput, TError1, TError2>(parser.Inner, errorMapper)
        );
    }

    // public static SetErrorParserBox<TToken, TOutVal, TError1, TError2> SetErrorWith<TToken, TOutVal, TError1, TError2>(
    //     this IParserBox<TToken, TOutVal, TError1> parserBox, Func<TError2> errorFunc)
    // {
    //     return new SetErrorParserBox<TToken, TOutVal, TError1, TError2>(parserBox, ValueSource<TError2>.Func(errorFunc));
    // }

    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, TError1, Unit>, TToken, TOutput, Unit>
        IgnoreError<TParser, TToken, TOutput, TError1>(
            this Parser<TParser, TToken, TOutput, TError1> parser)
        where TParser : struct, IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<MapErrorParser<TParser, TToken, TOutput, TError1, Unit>, TToken, TOutput, Unit>(
            new MapErrorParser<TParser, TToken, TOutput, TError1, Unit>(parser.Inner, _ => new Unit())
        );
    }

    public static
        Parser<
            MapParser<TParser, TToken, TOutput1, TOutput2, TError>, 
            TToken, TOutput2, TError
        >
        Map<TParser, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser, TToken, TOutput1, TError> parser, 
            Func<TOutput1, TOutput2> mapper
        )
        where TParser : struct, IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<MapParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new MapParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<MapIParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
        MapI<TParser, TToken, TOutput, TError>(
            this Parser<TParser, TToken, InputRange, TError> parser, 
            SpanFunc<TToken, TOutput> mapper
        )
        where TParser : struct, IParser<TParser, TToken, InputRange, TError>
    {
        return new Parser<MapIParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>(
            new MapIParser<TParser, TToken, TOutput, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TError>, TToken, TOutput, TError>
        EndsWith<TParser, TEndParser, TToken, TOutput, TError>(
            this Parser<TParser, TToken, TOutput, TError> parser, 
            Parser<TEndParser, TToken, Unit, TError> endParser
        )
        where TParser : struct, IParser<TParser, TToken, TOutput, TError>
        where TEndParser : struct, IParser<TEndParser, TToken, Unit, TError>
    {
        return new Parser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TError>, TToken, TOutput, TError>(
            new EndsWithParser<TParser, TEndParser, TToken, TOutput, TError>(parser.Inner, endParser.Inner)
        );
    }

    public static
        Parser<BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        Bind<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser1, TToken, TOutput1, TError> parser,
            Func<TOutput1, Parser<TParser2, TToken, TOutput2, TError>> binder)
        where TParser1 : struct, IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : struct, IParser<TParser2, TToken, TOutput2, TError>
    {
        return new Parser<BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(parser.Inner, BinderUnwrap)
        );

        TParser2 BinderUnwrap(TOutput1 o1)
        {
            return binder(o1).Inner;
        }
    }
}
