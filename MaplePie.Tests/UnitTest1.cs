using System.Buffers;
using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;
using Xunit.Abstractions;

namespace MaplePie.Tests;


public class Pg
{
    private readonly ITestOutputHelper _output;

    public Pg(ITestOutputHelper output)
    {
        _output = output;
    }

    
    
    [Fact]
    public void Tag()
    {
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

    public static ParseResult<char, string, Unit> TestParserFunc(Span<char> input)
    {
        var countParser = 
            Parsers.TakeWhile<char>(char.IsDigit)
                .MapI(cs => int.Parse(cs))
                .IgnoreError();
        var delimiterParser = Parsers.Tag(">>=").IgnoreError();
        var sublineParserFactory =
            (int c) => Parsers.Take<char>(c).IgnoreError();

        var countRes = countParser.Parse(input, 0);
        if (!countRes.IsOk) return countRes.Anything<string>();

        var delimRes = delimiterParser.Parse(input, countRes.Position);
        if (!delimRes.IsOk) return delimRes.Anything<string>();

        var sublineParser = sublineParserFactory(countRes.Output);
        var sublineRes = sublineParser.Parse(input, countRes.Position);
        if (!sublineRes.IsOk) return sublineRes.Anything<string>();

        var subline = sublineRes.InputOutput(input);

        return ParseResult<char, string, Unit>.Ok(sublineRes.Position, new string(subline));
    }
    
    [Fact]
    public void Bind()
    {
        var input = "7>>=123abcFh";

        var countParser =
            Parsers.TakeWhile<char>(char.IsDigit).SetContext("Count")
                .EndsWith(Parsers.Tag(">>=").SetContext("Delimiter"))
                .MapI(cs => int.Parse(cs))
                .AddContext("Count-Delimiter");
        
        var sublineParser =
            (int c) => Parsers.Take<char>(c).SetContext("Subline");

        var parser =
            countParser.Bind(c =>
                sublineParser(c)
                    .EndsWith(Parsers.Eof<char>().SetContext("Eof"))
                    .MapI(span => new string(span))
                ).AddContext("Root");

        var r = parser.BeginParse(input.AsSpan());
        // r.RemainderRange.CursorRange.IsEmpty

        _output.WriteLine($"{r}");
        Assert.Equal(input.Length, r.Position);
    }
}