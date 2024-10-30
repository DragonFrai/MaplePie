using System.Text.Json.Nodes;
using MaplePie.Errors;
using MaplePie.Parser;
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
}

public class JsonBool(bool value) : IJsonValue
{
    public JsonValueKind Kind => JsonValueKind.Bool;
    public bool Value = value;
}

public class JsonString(string value) : IJsonValue
{
    public JsonValueKind Kind => JsonValueKind.String;
    public string Value = value;
}

public class JsonExample
{
    private readonly ITestOutputHelper _output;

    public JsonExample(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Json()
    {

        var nullParser = Parsers.Tag("null".AsMemory()).ToBoxedParser();

        var boolParser = Parsers.Select(
            Parsers.Tag("true".AsMemory()).Map(_ => true), 
            Parsers.Tag("false".AsMemory()).Map(_ => false)
        ).ToBoxedParser();

        // var notEscapedString =
        //         Parsers.Tag("\"".AsMemory())
        //             .Bind(_ => Parsers.TakeWhile<char>(c => c != '"'))
        //             .EndsWith(Parsers.Tag("\"".AsMemory()).Map(_ => Unit.Instance))
        //             .MapI(span => span.ToString())
        //     ;

        var stringChar =
            Parsers.TokenIs((char c) => c != '"')
                .Map(_ => Unit.Instance)
                .Or(Parsers.Tag("\\\"").Map(_ => Unit.Instance))
                .Or(Parsers.Tag("\n").Map(_ => Unit.Instance))
                .Or(Parsers.Tag("\\\\").Map(_ => Unit.Instance));
        
        var escapedString =
                Parsers.Tag("\"".AsMemory())
                    .Bind(_ => Parsers.TakeWhileP(stringChar))
                    .EndsWith(Parsers.Tag("\"".AsMemory()))
                    .MapI(span => span.ToString());

        // var number =
        //     

        var valueParser =
            nullParser.Map(_ => (IJsonValue) new JsonNull())
                .Or(boolParser.Map(v => (IJsonValue) new JsonBool(v)))
                .Or(escapedString.Map(v => (IJsonValue) new JsonString(v)))
                .EndsWith(Parsers.Eof<char>());

        var input = "\"true\"".AsSpan();
        var r = valueParser.BeginParse(input);
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