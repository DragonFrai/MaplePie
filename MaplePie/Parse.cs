using MaplePie.Parsers.Combine;
using MaplePie.Parser;
using MaplePie.Parsers.Branch;
using MaplePie.Parsers.Combine;
using MaplePie.Parsers.Multi;
using MaplePie.Parsers.Sequence;
using MaplePie.Parsers.Token;
using MaplePie.Parsers.Zero;
using MaplePie.Utils;

namespace MaplePie;

public static class Parse
{
    // [ ZeroParsers ]
    
    public static Parser<OkParser<TToken, TOutput, TError>, TToken, TOutput, TError> Ok<TToken, TOutput, TError>(TOutput output)
    {
        return new Parser<OkParser<TToken, TOutput, TError>, TToken, TOutput, TError>(new OkParser<TToken, TOutput, TError>(output));
    }

    public static Parser<MissParser<TToken, TOutput, TError>, TToken, TOutput, TError> Miss<TToken, TOutput, TError>(TError error)
    {
        return new Parser<MissParser<TToken, TOutput, TError>, TToken, TOutput, TError>(new MissParser<TToken, TOutput, TError>(error));
    }
    
    public static Parser<FailParser<TToken, TOutput, TError>, TToken, TOutput, TError> Fail<TToken, TOutput, TError>(TError error)
    {
        return new Parser<FailParser<TToken, TOutput, TError>, TToken, TOutput, TError>(new FailParser<TToken, TOutput, TError>(error));
    }
    
    
    // [ Skip ]
    
    public static Parser<SkipParser<TToken>, TToken, Unit, Unit> Skip<TToken>(int count)
    {
        return new Parser<SkipParser<TToken>, TToken, Unit, Unit>(new SkipParser<TToken>(count));
    }

    public static Parser<SkipEqParser<TToken>, TToken, Unit, Unit> SkipEq<TToken>(TToken token)
    {
        return new Parser<SkipEqParser<TToken>, TToken, Unit, Unit>(new SkipEqParser<TToken>(token));
    }

    public static Parser<SkipIfParser<TToken>, TToken, Unit, Unit> SkipIf<TToken>(Func<TToken, bool> cond)
    {
        return new Parser<SkipIfParser<TToken>, TToken, Unit, Unit>(new SkipIfParser<TToken>(cond));
    }

    public static Parser<SkipSeqParser<TToken>, TToken, Unit, Unit> SkipSeq<TToken>(ReadOnlyMemory<TToken> tokens)
    {
        return new Parser<SkipSeqParser<TToken>, TToken, Unit, Unit>(new SkipSeqParser<TToken>(tokens));
    }
    
    public static Parser<SkipSeqParser<char>, char, Unit, Unit> SkipSeq(string tokens)
    {
        return SkipSeq(tokens.AsMemory());
    }
    
    public static Parser<SkipWhileParser<TToken>, TToken, Unit, Unit> SkipWhile<TToken>(Func<TToken, bool> cond)
    {
        return new Parser<SkipWhileParser<TToken>, TToken, Unit, Unit>(new SkipWhileParser<TToken>(cond));
    }
    
    
    // [ Take ]
    
    public static Parser<TakeParser<TToken>, TToken, InputRange, Unit> Take<TToken>(int count)
    {
        return new Parser<TakeParser<TToken>, TToken, InputRange, Unit>(new TakeParser<TToken>(count: count));
    }

    public static Parser<TakeEqParser<TToken>, TToken, InputRange, Unit> TakeEq<TToken>(TToken token)
    {
        return new Parser<TakeEqParser<TToken>, TToken, InputRange, Unit>(new TakeEqParser<TToken>(token));
    }

    public static Parser<TakeIfParser<TToken>, TToken, InputRange, Unit> TakeIf<TToken>(Func<TToken, bool> cond)
    {
        return new Parser<TakeIfParser<TToken>, TToken, InputRange, Unit>(new TakeIfParser<TToken>(cond));
    }

    public static Parser<TakeSeqParser<TToken>, TToken, InputRange, Unit> TakeSeq<TToken>(ReadOnlyMemory<TToken> tokens)
    {
        return new Parser<TakeSeqParser<TToken>, TToken, InputRange, Unit>(new TakeSeqParser<TToken>(tokens));
    }
    
    public static Parser<TakeSeqParser<char>, char, InputRange, Unit> TakeSeq(string tokens)
    {
        return TakeSeq(tokens.AsMemory());
    }
    
    public static Parser<TakeWhileParser<TToken>, TToken, InputRange, Unit> TakeWhile<TToken>(Func<TToken, bool> cond)
    {
        return new Parser<TakeWhileParser<TToken>, TToken, InputRange, Unit>(new TakeWhileParser<TToken>(cond));
    }
    
    
    // [ Token ]
    
    public static Parser<TokenParser<TToken>, TToken, TToken, Unit> Token<TToken>()
    {
        return new Parser<TokenParser<TToken>, TToken, TToken, Unit>(new TokenParser<TToken>());
    }

    public static Parser<TokenEqParser<TToken>, TToken, TToken, Unit> TokenEq<TToken>(TToken token)
    {
        return new Parser<TokenEqParser<TToken>, TToken, TToken, Unit>(new TokenEqParser<TToken>(token));
    }

    public static Parser<TokenIsParser<TToken>, TToken, TToken, Unit> TokenIs<TToken>(Func<TToken, bool> cond)
    {
        return new Parser<TokenIsParser<TToken>, TToken, TToken, Unit>(new TokenIsParser<TToken>(cond));
    }

    public static Parser<TokenSeqParser<TToken>, TToken, ReadOnlyMemory<TToken>, Unit> TokenSeq<TToken>(ReadOnlyMemory<TToken> tokens)
    {
        return new Parser<TokenSeqParser<TToken>, TToken, ReadOnlyMemory<TToken>, Unit>(new TokenSeqParser<TToken>(tokens));
    }
    
    public static Parser<TokenSeqParser<char>, char, ReadOnlyMemory<char>, Unit> TokenSeq(string tokens)
    {
        return new Parser<TokenSeqParser<char>, char, ReadOnlyMemory<char>, Unit>(new TokenSeqParser<char>(tokens.AsMemory()));
    }
    
    public static Parser<IsEndParser<TToken>, TToken, Unit, Unit> End<TToken>()
    {
        return new Parser<IsEndParser<TToken>, TToken, Unit, Unit>(new IsEndParser<TToken>());
    }
    
    // [ Branches ]

    public static
        Parser<Alt2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        Alt<TParser1, TParser2, TToken, TOutput, TError>(
            Parser<TParser1, TToken, TOutput, TError> parser1, 
            Parser<TParser2, TToken, TOutput, TError> parser2,
            ValueSource<TError> notSelectedError) 
        where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, TError>
    {
        return new Parser<Alt2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        (
            new Alt2Parser<TParser1, TParser2, TToken, TOutput, TError>
            (
                parser1.Inner, 
                parser2.Inner,
                notSelectedError
            )
        );
    }
    
    public static
        Parser<Alt2Parser<TParser1, TParser2, TToken, TOutput, Unit>, TToken, TOutput, Unit>
        Alt<TParser1, TParser2, TToken, TOutput>(
            Parser<TParser1, TToken, TOutput, Unit> parser1, 
            Parser<TParser2, TToken, TOutput, Unit> parser2)
        where TParser1 : IParser<TParser1, TToken, TOutput, Unit> 
        where TParser2 : IParser<TParser2, TToken, TOutput, Unit>
    {
        return new Parser<Alt2Parser<TParser1, TParser2, TToken, TOutput, Unit>, TToken, TOutput, Unit>
        (
            new Alt2Parser<TParser1, TParser2, TToken, TOutput, Unit>
            (
                parser1.Inner, 
                parser2.Inner,
                ValueSource<Unit>.Value(Unit.Instance)
            )
        );
    }
    
    public static
        Parser<AltBoxedParser<TToken, TOutput, TError>, TToken, TOutput, TError>
        AltBoxed<TToken, TOutput, TError>(BoxedParser<TToken, TOutput, TError>[] alternatives, ValueSource<TError> notSelectedError)
    {
        return new Parser<AltBoxedParser<TToken, TOutput, TError>, TToken, TOutput, TError>
        (
            new AltBoxedParser<TToken, TOutput, TError>(alternatives, notSelectedError)
        );
    }
    
    public static
        Parser<AltBoxedParser<TToken, TOutput, Unit>, TToken, TOutput, Unit>
        AltBoxed<TToken, TOutput>(BoxedParser<TToken, TOutput, Unit>[] alternatives)
    {
        return new Parser<AltBoxedParser<TToken, TOutput, Unit>, TToken, TOutput, Unit>
        (
            new AltBoxedParser<TToken, TOutput, Unit>(alternatives, ValueSource<Unit>.Value(Unit.Instance))
        );
    }

    public static
        Parser<IfElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        IfElse<TParser1, TParser2, TToken, TOutput, TError>(
            bool condition,
            Parser<TParser1, TToken, TOutput, TError> parserIf,
            Parser<TParser2, TToken, TOutput, TError> parserElse
        ) 
        where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, TError>
    {
        return new Parser<IfElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        (
            new IfElseParser<TParser1, TParser2, TToken, TOutput, TError>(condition, parserIf.Inner, parserElse.Inner)
        );
    }

    public static
        Parser<OrElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        OrElse<TParser1, TParser2, TToken, TOutput, TError>(
            Parser<TParser1, TToken, TOutput, TError> parser,
            Parser<TParser2, TToken, TOutput, TError> parserAlt
        ) 
        where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, TError>
    {
        return new Parser<OrElseParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        (
            new OrElseParser<TParser1, TParser2, TToken, TOutput, TError>(parser.Inner, parserAlt.Inner)
        );
    }

    public static
        Parser<IfParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
        Condition<TParser, TToken, TOutput, TError>(
            bool condition,
            Parser<TParser, TToken, TOutput, TError> parser,
            ValueSource<TError> error) 
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<IfParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
        (
            new IfParser<TParser, TToken, TOutput, TError>(condition, parser.Inner, error)
        );
    }

    public static
        Parser<IfParser<TParser, TToken, TOutput, Unit>, TToken, TOutput, Unit>
        Condition<TParser, TToken, TOutput>(
            bool condition,
            Parser<TParser, TToken, TOutput, Unit> parser) 
        where TParser : IParser<TParser, TToken, TOutput, Unit>
    {
        return new Parser<IfParser<TParser, TToken, TOutput, Unit>, TToken, TOutput, Unit>
        (
            new IfParser<TParser, TToken, TOutput, Unit>(condition, parser.Inner, ValueSource<Unit>.Value(Unit.Instance))
        );
    }
    
    // [ Combine ]


    public static
        Parser<LazyParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>
        Lazy<TParser, TToken, TOutput, TError>(
            Func<Parser<TParser, TToken, TOutput, TError>> parserFunc
        )
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<LazyParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>(
            new LazyParser<TParser, TToken, TOutput, TError>(() => parserFunc().Inner)
        );
    }

    public static
        Parser<BindParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        Bind<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser1, TToken, TOutput1, TError> parser,
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
        Parser<MapValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        Map<TParser, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser, TToken, TOutput1, TError> parser,
            Func<TOutput1, TOutput2> mapper)
        where TParser : IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<MapValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new MapValueParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, mapper)
        );
    }

    public static
        Parser<MapValueWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        MapI<TParser, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser, TToken, TOutput1, TError> parser,
            SpanArgFunc<TToken, TOutput1, TOutput2> mapper)
        where TParser : IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<MapValueWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new MapValueWithInputParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, mapper)
        );
    }

    public static
        Parser<MapValueInputSpanParser<TParser, TToken, TOutput2, TError>, TToken, TOutput2, TError>
        MapS<TParser, TToken, TOutput2, TError>(
            Parser<TParser, TToken, InputRange, TError> parser,
            SpanFunc<TToken, TOutput2> mapper)
        where TParser : IParser<TParser, TToken, InputRange, TError>
    {
        return new Parser<MapValueInputSpanParser<TParser, TToken, TOutput2, TError>, TToken, TOutput2, TError>(
            new MapValueInputSpanParser<TParser, TToken, TOutput2, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
        MapError<TParser, TToken, TOutput, TError1, TError2>(
            Parser<TParser, TToken, TOutput, TError1> parser,
            Func<TError1, TError2> mapper)
        where TParser : IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<MapErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>(
            new MapErrorParser<TParser, TToken, TOutput, TError1, TError2>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        Value<TParser, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser, TToken, TOutput1, TError> parser,
            TOutput2 value)
        where TParser : IParser<TParser, TToken, TOutput1, TError>
    {
        return new Parser<SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new SetValueParser<TParser, TToken, TOutput1, TOutput2, TError>(parser.Inner, value)
        );
    }
    
    public static
        Parser<SetErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>
        Error<TParser, TToken, TOutput, TError1, TError2>(
            Parser<TParser, TToken, TOutput, TError1> parser,
            TError2 error)
        where TParser : IParser<TParser, TToken, TOutput, TError1>
    {
        return new Parser<SetErrorParser<TParser, TToken, TOutput, TError1, TError2>, TToken, TOutput, TError2>(
            new SetErrorParser<TParser, TToken, TOutput, TError1, TError2>(parser.Inner, error)
        );
    }
    
    public static
        Parser<ConsumedParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>
        Consumed<TParser, TToken, TOutput, TError>(
            Parser<TParser, TToken, TOutput, TError> parser)
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<ConsumedParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>(
            new ConsumedParser<TParser, TToken, TOutput, TError>(parser.Inner)
        );
    }
    
    public static
        Parser<OptionalParser<TParser, NullableOptionProxy<TOutput>, TToken, TOutput, TOutput?, TError>, TToken, TOutput?, TError>
        Opt<TParser, TToken, TOutput, TError>(
            Parser<TParser, TToken, TOutput, TError> parser)
        where TParser : IParser<TParser, TToken, TOutput, TError>
        where TOutput : class
    {
        return new Parser<OptionalParser<TParser, NullableOptionProxy<TOutput>, TToken, TOutput, TOutput?, TError>, TToken, TOutput?, TError>(
            new OptionalParser<TParser, NullableOptionProxy<TOutput>, TToken, TOutput, TOutput?, TError>(parser.Inner)
        );
    }
    
    public static
        Parser<OptionalParser<TParser, ValueNullableOptionProxy<TOutput>, TToken, TOutput, TOutput?, TError>, TToken, TOutput?, TError>
        ValueOpt<TParser, TToken, TOutput, TError>(
            Parser<TParser, TToken, TOutput, TError> parser)
        where TParser : IParser<TParser, TToken, TOutput, TError>
        where TOutput : struct
    {
        return new Parser<OptionalParser<TParser, ValueNullableOptionProxy<TOutput>, TToken, TOutput, TOutput?, TError>, TToken, TOutput?, TError>(
            new OptionalParser<TParser, ValueNullableOptionProxy<TOutput>, TToken, TOutput, TOutput?, TError>(parser.Inner)
        );
    }
    
    // [ Sequence ]

    public static
        Parser<Tuple2Parser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, (TOutput1, TOutput2),
            TError>
        Tuple<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser1, TToken, TOutput1, TError> parser1,
            Parser<TParser2, TToken, TOutput2, TError> parser2
        )
        where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
    {
        return new Parser<Tuple2Parser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, (TOutput1,
            TOutput2), TError>(
            new Tuple2Parser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(parser1.Inner, parser2.Inner)
        );
    }
    
    public static
        Parser<Tuple3Parser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>, TToken, (TOutput1, TOutput2, TOutput3),
            TError>
        Tuple<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>(
            Parser<TParser1, TToken, TOutput1, TError> parser1,
            Parser<TParser2, TToken, TOutput2, TError> parser2,
            Parser<TParser3, TToken, TOutput3, TError> parser3
        )
        where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
        where TParser3 : IParser<TParser3, TToken, TOutput3, TError>
    {
        return new Parser<Tuple3Parser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>, TToken, (TOutput1,
            TOutput2, TOutput3), TError>(
            new Tuple3Parser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>(parser1.Inner, parser2.Inner, parser3.Inner)
        );
    }
    
    
    public static
        Parser<BetweenParser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>, TToken, TOutput2,
            TError>
        Between<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>(
            Parser<TParser1, TToken, TOutput1, TError> parser1,
            Parser<TParser2, TToken, TOutput2, TError> parser2,
            Parser<TParser3, TToken, TOutput3, TError> parser3
        )
        where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
        where TParser3 : IParser<TParser3, TToken, TOutput3, TError>
    {
        return new Parser<BetweenParser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>, TToken,
            TOutput2, TError>(
            new BetweenParser<TParser1, TParser2, TParser3, TToken, TOutput1, TOutput2, TOutput3, TError>(parser1.Inner, parser2.Inner, parser3.Inner)
        );
    }
    
    public static
        Parser<PairLeftParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput1, TError>
        PairL<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser1, TToken, TOutput1, TError> parser1,
            Parser<TParser2, TToken, TOutput2, TError> parser2
        )
        where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
    {
        return new Parser<PairLeftParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput1, TError>(
            new PairLeftParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(parser1.Inner, parser2.Inner)
        );
    }

    public static
        Parser<PairRightParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>
        PairR<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(
            Parser<TParser1, TToken, TOutput1, TError> parser1,
            Parser<TParser2, TToken, TOutput2, TError> parser2
        )
        where TParser1 : IParser<TParser1, TToken, TOutput1, TError>
        where TParser2 : IParser<TParser2, TToken, TOutput2, TError>
    {
        return new Parser<PairRightParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>, TToken, TOutput2, TError>(
            new PairRightParser<TParser1, TParser2, TToken, TOutput1, TOutput2, TError>(parser1.Inner, parser2.Inner)
        );
    }


    


    // [ Multi ]
    
    public static
        Parser<FoldParser<TParser, TToken, TOutput, TState, TError>, TToken, TState, TError>
        Fold<TParser, TToken, TState, TOutput, TError>(
            Parser<TParser, TToken, TOutput, TError> parser, 
            Func<TState> init,
            Func<TState, TOutput, TState> folder)
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<FoldParser<TParser, TToken, TOutput, TState, TError>, TToken, TState, TError>
        (
            new FoldParser<TParser, TToken, TOutput, TState, TError>(parser.Inner, init, folder)
        );
    }
    
    // public static
    //     Parser<SkipWhilePParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>
    //     SkipWhileP<TParser, TToken, TOutput, TError>(Parser<TParser, TToken, TOutput, TError> parser)
    //     where TParser : IParser<TParser, TToken, TOutput, TError>
    // {
    //     return new Parser<SkipWhilePParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>(
    //         new SkipWhilePParser<TParser, TToken, TOutput, TError>(parser.Inner)
    //     );
    // }

    
    public static
        Parser<TakeWhilePParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>
        TakeWhileP<TParser, TToken, TOutput, TError>(Parser<TParser, TToken, TOutput, TError> parser)
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<TakeWhilePParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>(
            new TakeWhilePParser<TParser, TToken, TOutput, TError>(parser.Inner)
        );
    }


    public static

        Parser<SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError>, TToken, List<TOutput>, TError>
        SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError>(
            Parser<TParserSep, TToken, TOutputSep, TError> separator,
            Parser<TParser, TToken, TOutput, TError> parser
        )
        where TParserSep : IParser<TParserSep, TToken, TOutputSep, TError>
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        return new Parser<SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError>, TToken,
            List<TOutput>, TError>(
            new SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError>(separator.Inner, parser.Inner)
        );
    }
    
    
    
    // ============

    //
    //
    // public static Parser<TokenSeqParser<TToken>, TToken, ReadOnlyMemory<TToken>, Unit> Tag<TToken>(ReadOnlyMemory<TToken> tag)
    // {
    //     return new Parser<TokenSeqParser<TToken>, TToken, ReadOnlyMemory<TToken>, Unit>(
    //         new TokenSeqParser<TToken>(tag));
    // }
    //
    // public static Parser<TokenSeqParser<char>, char, ReadOnlyMemory<char>, Unit> Tag(string tag)
    // {
    //     return Tag(tag.AsMemory());
    // }
    //
    // public static Parser<TakeParser<TToken>, TToken, InputRange, Unit> Take<TToken>(int count)
    // {
    //     return new Parser<TakeParser<TToken>, TToken, InputRange, Unit>(
    //         new TakeParser<TToken>(count));
    // }
    //
    // public static
    //     Parser<TakeWhileParser<TToken>, TToken, InputRange, Unit>
    //     TakeWhile<TToken>(Func<TToken, bool> condition)
    // {
    //     return new Parser<TakeWhileParser<TToken>, TToken, InputRange, Unit>(
    //         new TakeWhileParser<TToken>(condition));
    // }
    //
    // public static
    //     Parser<TakeWhilePParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>
    //     TakeWhileP<TParser, TToken, TOutput, TError>(Parser<TParser, TToken, TOutput, TError> parser)
    //     where TParser : IParser<TParser, TToken, TOutput, TError>
    // {
    //     return new Parser<TakeWhilePParser<TParser, TToken, TOutput, TError>, TToken, InputRange, TError>(
    //         new TakeWhilePParser<TParser, TToken, TOutput, TError>(parser.Inner)
    //     );
    // }
    //
    // public static
    //     Parser<TokenIsParser<TToken>, TToken, TToken, Unit>
    //     TokenIs<TToken>(Func<TToken, bool> condition)
    // {
    //     return new Parser<TokenIsParser<TToken>, TToken, TToken, Unit>(
    //         new TokenIsParser<TToken>(condition));
    // }
    //
    // public static
    //     Parser<AlternativesParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
    //     Alt<TParser1, TParser2, TToken, TOutput, TError>(
    //         Parser<TParser1, TToken, TOutput, TError> parser1, 
    //         Parser<TParser2, TToken, TOutput, TError> parser2,
    //         ValueSource<TError> notSelectedError) 
    //     where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
    //     where TParser2 : IParser<TParser2, TToken, TOutput, TError>
    // {
    //     return new Parser<AlternativesParser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
    //     (
    //         new AlternativesParser<TParser1, TParser2, TToken, TOutput, TError>
    //         (
    //             parser1.Inner, 
    //             parser2.Inner,
    //             notSelectedError
    //         )
    //     );
    // }
    //
    // public static
    //     Parser<AlternativesParser<TParser1, TParser2, TToken, TOutput, Unit>, TToken, TOutput, Unit>
    //     Alt<TParser1, TParser2, TToken, TOutput>(
    //         Parser<TParser1, TToken, TOutput, Unit> parser1, 
    //         Parser<TParser2, TToken, TOutput, Unit> parser2)
    //     where TParser1 : IParser<TParser1, TToken, TOutput, Unit> 
    //     where TParser2 : IParser<TParser2, TToken, TOutput, Unit>
    // {
    //     return new Parser<AlternativesParser<TParser1, TParser2, TToken, TOutput, Unit>, TToken, TOutput, Unit>
    //     (
    //         new AlternativesParser<TParser1, TParser2, TToken, TOutput, Unit>
    //         (
    //             parser1.Inner, 
    //             parser2.Inner,
    //             ValueSource<Unit>.Value(Unit.Instance)
    //         )
    //     );
    // }
    //
    // public static
    //     Parser<FoldParser<TParser, TToken, TOutput, TState, TError>, TToken, TState, TError>
    //     Fold<TParser, TToken, TState, TOutput, TError>(
    //         Parser<TParser, TToken, TOutput, TError> parser, 
    //         Func<TState> init,
    //         Func<TState, TOutput, TState> folder)
    //     where TParser : IParser<TParser, TToken, TOutput, TError>
    // {
    //     return new Parser<FoldParser<TParser, TToken, TOutput, TState, TError>, TToken, TState, TError>
    //     (
    //         new FoldParser<TParser, TToken, TOutput, TState, TError>(parser.Inner, init, folder)
    //     );
    // }
    //
    // public static Parser<IsEndParser<TToken>, TToken, Unit, Unit> Eof<TToken>()
    // {
    //     return new Parser<IsEndParser<TToken>, TToken, Unit, Unit>(new IsEndParser<TToken>());
    // }
}

