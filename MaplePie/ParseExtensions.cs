using MaplePie.Parsers.Combine;
using MaplePie.Parser;
using MaplePie.Parsers.Branch;
using MaplePie.Parsers.Combine;
using MaplePie.Parsers.Multi;
using MaplePie.Parsers.Sequence;
using MaplePie.Utils;

namespace MaplePie;

public static class ParseExtensions
{
    
    public static
        Parser<SetValueParser<TParser, TToken, TOutput, TValue, TError>, TToken, TValue, TError>
        Value<TParser, TToken, TValue, TOutput, TError>(
            this Parser<TParser, TToken, TOutput, TError> parser, 
            TValue value)
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return Parse.Value(parser, value);
    }
    
    
    public static
        Parser<FoldParser<TParser, TToken, TOutput, TState, TError>, TToken, TState, TError>
        Fold<TParser, TToken, TState, TOutput, TError>(
            this Parser<TParser, TToken, TOutput, TError> parser, 
            Func<TState> init,
            Func<TState, TOutput, TState> folder)
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return Parse.Fold(parser, init, folder);
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
        where TParser : IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<MapErrorParser<TParser, TToken, TOutput, TError1, Unit>, TToken, TOutput, Unit>(
            new MapErrorParser<TParser, TToken, TOutput, TError1, Unit>(parser.Inner, _ => new Unit())
        );
    }

    public static
        Parser<
            MapValueParser<TParser, TToken, TOutput1, TOutput2, TError>, 
            TToken, TOutput2, TError
        >
        Map<TParser, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser, TToken, TOutput1, TError> parser, 
            Func<TOutput1, TOutput2> mapper
        )
        where TParser : IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<MapValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new MapValueParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<MapValueInputSpanParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
        MapS<TParser, TToken, TOutput, TError>(
            this Parser<TParser, TToken, InputRange, TError> parser, 
            SpanFunc<TToken, TOutput> mapper
        )
        where TParser : IParser<TParser, TToken, InputRange, TError>
    {
        return new Parser<MapValueInputSpanParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>(
            new MapValueInputSpanParser<TParser, TToken, TOutput, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<MapValueWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        MapI<TParser, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser, TToken, TOutput1, TError> parser, 
            SpanArgFunc<TToken, TOutput1, TOutput2> mapper
        )
        where TParser : IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<MapValueWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new MapValueWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
        MapError<TParser, TToken, TOutput, TError1, TError2>(
            this Parser<TParser, TToken, TOutput, TError1> parser, Func<TError1, TError2> errorMapper)
        where TParser : IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>(
            new MapErrorParser<TParser, TToken, TOutput, TError1, TError2>(parser.Inner, errorMapper)
        );
    }
    
    public static
        Parser<
            SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>, 
            TToken, TOutput2, TError
        >
        Set<TParser, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser, TToken, TOutput1, TError> parser, 
            TOutput2 value
        )
        where TParser : IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, value)
        );
    }
    
    public static
        Parser<SetErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
        SetError<TParser, TToken, TOutput, TError1, TError2>(
            this Parser<TParser, TToken, TOutput, TError1> parser, TError2 error)
        where TParser : IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<SetErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>(
            new SetErrorParser<TParser, TToken, TOutput, TError1, TError2>(parser.Inner, error)
        );
    }
    
    public static
        Parser<ConsumedParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>
        Consumed<TParser, TToken, TOutput, TError>(
            this Parser<TParser, TToken, TOutput, TError> parser)
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<ConsumedParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>(
            new ConsumedParser<TParser, TToken, TOutput, TError>(parser.Inner)
        );
    }

    
    //
    // public static
    //     Parser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>, TToken, TOutput, TError>
    //     EndsWith<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>(
    //         this Parser<TParser, TToken, TOutput, TError> parser, 
    //         Parser<TEndParser, TToken, TEndOutput, TError> endParser
    //     )
    //     where TParser : IParser<TParser, TToken, TOutput, TError>
    //     where TEndParser : IParser<TEndParser, TToken, TEndOutput, TError>
    // {
    //     return new Parser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>, TToken, TOutput, TError>(
    //         new EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>(parser.Inner, endParser.Inner)
    //     );
    // }

    public static
        Parser<BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        Bind<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser1, TToken, TOutput1, TError> parser,
            Func<TOutput1, Parser<TParser2, TToken, TOutput2, TError>> binder)
        where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
    {
        return new Parser<BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(parser.Inner, BinderUnwrap)
        );

        TParser2 BinderUnwrap(TOutput1 o1)
        {
            return binder(o1).Inner;
        }
    }
    
    
    public static
        Parser<OrElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        OrElse<TParser1, TParser2, TToken, TOutput, TError>(
            this Parser<TParser1, TToken, TOutput, TError> parser1, 
            Parser<TParser2, TToken, TOutput, TError> parser2) 
        where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, TError>
    {
        return Parse.OrElse(parser1, parser2);
    }
}
