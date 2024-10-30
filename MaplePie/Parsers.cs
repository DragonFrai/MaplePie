using System.Buffers;
using System.Runtime.InteropServices;
using MaplePie.BaseParsers;
using MaplePie.BranchParsers;
using MaplePie.CombineParsers;
using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie;

public static class Parsers
{
    public static Parser<TagParser<TToken>, TToken, InputRange, ParseError> Tag<TToken>(ReadOnlyMemory<TToken> tag)
    {
        return new Parser<TagParser<TToken>, TToken, InputRange, ParseError>(
            new TagParser<TToken>(tag));
    }

    public static Parser<TagParser<char>, char, InputRange, ParseError> Tag(string tag)
    {
        return Tag(tag.AsMemory());
    }

    public static Parser<TakeParser<TToken>, TToken, InputRange, ParseError> Take<TToken>(int count)
    {
        return new Parser<TakeParser<TToken>, TToken, InputRange, ParseError>(
            new TakeParser<TToken>(count));
    }

    public static
        Parser<TakeWhileParser<TToken>, TToken, InputRange, ParseError>
        TakeWhile<TToken>(Func<TToken, bool> condition)
    {
        return new Parser<TakeWhileParser<TToken>, TToken, InputRange, ParseError>(
            new TakeWhileParser<TToken>(condition));
    }
    
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
        Parser<TokenIsParser<TToken>, TToken, TToken, ParseError>
        TokenIs<TToken>(Func<TToken, bool> condition)
    {
        return new Parser<TokenIsParser<TToken>, TToken, TToken, ParseError>(
            new TokenIsParser<TToken>(condition));
    }

    public static
        Parser<Select2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        Select<TParser1, TParser2, TToken, TOutput, TError>(
            Parser<TParser1, TToken, TOutput, TError> parser1, 
            Parser<TParser2, TToken, TOutput, TError> parser2,
            ValueSource<TError> notSelectedError) 
        where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, TError>
    {
        return new Parser<Select2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        (
            new Select2Parser<TParser1, TParser2, TToken, TOutput, TError>
            (
                parser1.Inner, 
                parser2.Inner,
                notSelectedError
            )
        );
    }
    
    public static
        Parser<Select2Parser<TParser1, TParser2, TToken, TOutput, ParseError>, TToken, TOutput, ParseError>
        Select<TParser1, TParser2, TToken, TOutput>(
            Parser<TParser1, TToken, TOutput, ParseError> parser1, 
            Parser<TParser2, TToken, TOutput, ParseError> parser2)
        where TParser1 : IParser<TParser1, TToken, TOutput, ParseError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, ParseError>
    {
        return new Parser<Select2Parser<TParser1, TParser2, TToken, TOutput, ParseError>, TToken, TOutput, ParseError>
        (
            new Select2Parser<TParser1, TParser2, TToken, TOutput, ParseError>
            (
                parser1.Inner, 
                parser2.Inner,
                ValueSource<ParseError>.Value(ParseError.Select)
            )
        );
    }

    public static Parser<EofParser<TToken>, TToken, Unit, ParseError> Eof<TToken>()
    {
        return new Parser<EofParser<TToken>, TToken, Unit, ParseError>(new EofParser<TToken>());
    }
}

public delegate TOutput SpanFunc<TToken, out TOutput>(ReadOnlySpan<TToken> span);
public delegate TOutput SpanArgFunc<TToken, in TArg, out TOutput>(ReadOnlySpan<TToken> span, TArg arg);

public static class ParserExtensions
{
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
            MapParser<TParser, TToken, TOutput1, TOutput2, TError>, 
            TToken, TOutput2, TError
        >
        Map<TParser, TToken, TOutput1, TOutput2, TError>(
            this Parser<TParser, TToken, TOutput1, TError> parser, 
            Func<TOutput1, TOutput2> mapper
        )
        where TParser : IParser<TParser, TToken, TOutput1, TError>
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
        where TParser : IParser<TParser, TToken, InputRange, TError>
    {
        return new Parser<MapIParser<TParser, TToken, TOutput, TError>, TToken, TOutput, TError>(
            new MapIParser<TParser, TToken, TOutput, TError>(parser.Inner, mapper)
        );
    }
    
    public static
        Parser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>, TToken, TOutput, TError>
        EndsWith<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>(
            this Parser<TParser, TToken, TOutput, TError> parser, 
            Parser<TEndParser, TToken, TEndOutput, TError> endParser
        )
        where TParser : IParser<TParser, TToken, TOutput, TError>
        where TEndParser : IParser<TEndParser, TToken, TEndOutput, TError>
    {
        return new Parser<EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>, TToken, TOutput, TError>(
            new EndsWithParser<TParser, TEndParser, TToken, TOutput, TEndOutput, TError>(parser.Inner, endParser.Inner)
        );
    }

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
        Parser<Select2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
        Or<TParser1, TParser2, TToken, TOutput, TError>(
            this Parser<TParser1, TToken, TOutput, TError> parser1, 
            Parser<TParser2, TToken, TOutput, TError> parser2,
            ValueSource<TError> notSelectedError) 
        where TParser1 : IParser<TParser1, TToken, TOutput, TError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, TError>

    {
        return Parsers.Select(parser1, parser2, notSelectedError);
    }
    
    public static
        Parser<Select2Parser<TParser1, TParser2, TToken, TOutput, ParseError>, TToken, TOutput, ParseError>
        Or<TParser1, TParser2, TToken, TOutput>(
            this Parser<TParser1, TToken, TOutput, ParseError> parser1, 
            Parser<TParser2, TToken, TOutput, ParseError> parser2) 
        where TParser1 : IParser<TParser1, TToken, TOutput, ParseError> 
        where TParser2 : IParser<TParser2, TToken, TOutput, ParseError>
    {
        return Parsers.Select(parser1, parser2);
    }
}
