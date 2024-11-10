using System.Text.Json.Nodes;
using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Parsers.Token;
using MaplePie.Utils;
using Xunit.Abstractions;

namespace MaplePie.Tests;


//#[derive(Debug, PartialEq, Clone)]
// pub enum JsonValue {
//   Null,
//   Bool(bool),
//   Str(String),
//   Num(f64),
//   Array(Vec<JsonValue>),
//   Object(HashMap<String, JsonValue>),
// }

public enum JsonValueKind
{
    Null,
    Bool,
    String,
    Number,
    Array,
    Object
}

public interface IJsonValue
{
    public abstract JsonValueKind Kind { get; }
}

public class JsonNull : IJsonValue
{
    public JsonValueKind Kind => JsonValueKind.Null;

    public override string ToString()
    {
        return "null";
    }
}

public class JsonBool(bool value) : IJsonValue
{
    public JsonValueKind Kind => JsonValueKind.Bool;
    public bool Value = value;

    public override string ToString()
    {
        return Value ? "true" : "false";
    }
}

public class JsonString(string value) : IJsonValue
{
    public JsonValueKind Kind => JsonValueKind.String;
    public string Value = value;

    public override string ToString()
    {
        return $"\"{Value}\"";
    }
}

public class JsonArray(IJsonValue[] values) : IJsonValue
{
    public JsonValueKind Kind => JsonValueKind.Array;
    public IJsonValue[] Values = values;

    public override string ToString()
    {
        var inner = string.Join<IJsonValue>(", ", Values);
        return $"[ {inner} ]";
    }
}

public class JsonExample
{
    private readonly ITestOutputHelper _output;

    public JsonExample(ITestOutputHelper output)
    {
        _output = output;
    }


    public static Parser<SkipWhileParser<char>, char, Unit, Unit> Whitespace0()
    {
        return Parse.SkipWhile((char c) => c is ' ' or '\n' or '\r' or '\t');
    }

    // public static Lazy<BoxedParser<char, IJsonValue, Unit>> JsonParser = new(CreateJsonParser);
    public static BoxedParser<char, IJsonValue, ContextError> JsonValueParser = CreateJsonParser();
    public static BoxedParser<char, IJsonValue, ContextError> JsonRootParser =
        Parse.PairL(
            Parse.Between(Whitespace0().SetContext("ws"), JsonValueParser.ToParser(), Whitespace0().SetContext("ws")),
            Parse.End<char>().SetContext("Expected EOF")
        ).ToBoxed();
    private static BoxedParser<char, IJsonValue, ContextError> CreateJsonParser()
    {
        // [ Utility ]

        var takeStringChar =
            Parse.Alt(
                Parse.Tuple(
                    Parse.SkipEq('\\'),
                    Parse.Alt(
                        Parse.SkipEq('\n'),
                        Parse.Alt(
                            Parse.SkipEq('\"'),
                            Parse.SkipEq('\\')
                        )
                    )
                ).Set(Unit.Instance),
                Parse.SkipIf((char c) => c != '"')
            );
    
        // [ Json ]
        
        var nullParser = Parse.SkipSeq("null").SetContext("null");

        var boolParser = Parse.Between(
            Whitespace0().SetContext("ws"),
            Parse.Alt(
                Parse.SkipSeq("true").Set(true), 
                Parse.SkipSeq("false").Set(false)
            ).SetContext("bool"), 
            Whitespace0().SetContext("ws"));
        
        var escapedString =
            Parse.Between(
                    Parse.SkipEq('\"'),
                    Parse.TakeWhileP(takeStringChar),
                    Parse.SkipEq('\"')
                )
                .MapS(span => span.ToString())
                .SetContext("string");
        
        var arrayParser =
            Parse.Between(
                Parse.SkipEq('[').SetContext("open"),
                Parse.SeparatedList0(
                    Parse.SkipEq(',').SetContext("sep"),
                    Parse.Lazy(() => JsonValueParser.ToParser())
                ),
                Parse.SkipEq(']').SetContext("close")
            ).AddContext("array");

        var valueParser =
                Parse.Alt(
                    Parse.Alt(
                        nullParser.Map(_ => (IJsonValue)new JsonNull()),
                        boolParser.Map(v => (IJsonValue)new JsonBool(v)),
                        ValueSource<ContextError>.Func(() => new ContextError("alt-null-bool"))
                    ),
                    Parse.Alt(
                        escapedString.Map(v => (IJsonValue)new JsonString(v)),
                        arrayParser.Map(vs => (IJsonValue)new JsonArray(vs.ToArray())),
                        ValueSource<ContextError>.Func(() => new ContextError("alt-string-array"))
                    ),
                    ValueSource<ContextError>.Func(() => new ContextError("alt"))
                );

        var wsValueParser = Parse.Between(Whitespace0().SetContext("ws"), valueParser, Whitespace0().SetContext("ws"));

        return wsValueParser.ToBoxed();
    }
    
    [Fact]
    public void Json()
    {



        // var input = " \n  [ true, \"null\"  , null, [], [\"hello\\t!\"] ]  ".AsSpan();
        var input = " [  true, \n \"null\"   \n, \t null, [], [ \t], [ false ], \"\\t\" ]  ".AsSpan();
        var parser = JsonRootParser.ToParser();
        var r = parser.Parse(input);
        _output.WriteLine(r.ToString(input));

        // var input = "abcdef".ToCharArray();
        // var tag = "abc".ToCharArray();
        //
        // var parser = Parsers.Tag(tag);
        //
        // var inputSeq = MemorySegment<char>.ChunkedSequence(input, 1);
        // var r = parser.Parse(new SequenceInput<char>(inputSeq, true));
        //
        // _output.WriteLine(r.ToString());
        //
        // var remains = inputSeq.Slice(r.Position);
        // Assert.Equal(3, remains.Length);


    }
}