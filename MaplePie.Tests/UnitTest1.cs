using System.Buffers;
using MaplePie.Utils;
using Xunit.Abstractions;

namespace MaplePie.Tests;


internal class MemorySegment<T> : ReadOnlySequenceSegment<T>
{
    public MemorySegment(ReadOnlyMemory<T> memory)
    {
        Memory = memory;
    }

    public int Length => Memory.Length;
    
    public MemorySegment<T> Append(ReadOnlyMemory<T> memory)
    {
        var segment = new MemorySegment<T>(memory)
        {
            RunningIndex = RunningIndex + Memory.Length
        };

        Next = segment;

        return segment;
    }
    
    
    public static ReadOnlySequence<T> ChunkedSequence(T[] source, int windowSize)
    {
        MemorySegment<T>? beginSegment = null;
        MemorySegment<T>? endSegment = null;
        
        var chunks = source.Chunk(windowSize);
        foreach (var chunk in chunks)
        {
            if (beginSegment == null)
            {
                beginSegment = new MemorySegment<T>(chunk.AsMemory());
                endSegment = beginSegment;
            }
            else
            {
                endSegment = endSegment?.Append(chunk.AsMemory());
            }
        }

        var ros = new ReadOnlySequence<T>(beginSegment!, 0, endSegment!, endSegment!.Length);

        return ros;
    }
}

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
        var delimiterParser = Parsers.Tag(['>', '>', '=']).IgnoreError();
        var sublineParserFactory =
            (int c) => Parsers.Take<char>(c).IgnoreError();

        var countRes = countParser.Parse(input, new InputRange(0, input.Length));
        if (!countRes.IsOk) return countRes.Anything<string>();

        var delimRes = delimiterParser.Parse(input, countRes.RemainderRange);
        if (!delimRes.IsOk) return delimRes.Anything<string>();

        var sublineParser = sublineParserFactory(countRes.Output);
        var sublineRes = sublineParser.Parse(input, countRes.RemainderRange);
        if (!sublineRes.IsOk) return sublineRes.Anything<string>();

        var subline = sublineRes.InputOutput(input);

        return ParseResult<char, string, Unit>.Ok(sublineRes.Output, new string(subline));
    }
    
    [Fact]
    public void Bind()
    {
        var input = "7>>=123abcFh";

        var countParser =
            Parsers.TakeWhile<char>(char.IsDigit)
                .IgnoreError()
                .EndsWith(Parsers.Tag(['>', '>', '=']).IgnoreError().Map(_ => new Unit()))
                .MapI(cs => int.Parse(cs));
        
        var sublineParser =
            (int c) => Parsers.Take<char>(c).IgnoreError();

        var parser =
            countParser.Bind(c =>
                sublineParser(c)
                    .EndsWith(Parsers.Eof<char>().IgnoreError())
                    .MapI(span => new string(span))
                );

        var r = parser.BeginParse(input.AsSpan());
        // r.RemainderRange.CursorRange.IsEmpty

        _output.WriteLine($"{r}");
        Assert.Equal(0, r.RemainderRange.Length);
    }
}