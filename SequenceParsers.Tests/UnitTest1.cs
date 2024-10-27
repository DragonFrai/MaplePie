using System.Buffers;
using SequenceParsers;
using SequenceParsers.Combinators;
using Xunit.Abstractions;

namespace SequenceParse.Tests;


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
        var input = "abcdef".ToCharArray();
        var tag = "abc".ToCharArray();

        var parser = Parsers.Tag<char>(tag);

        var inputSeq = MemorySegment<char>.ChunkedSequence(input, 1);
        var r = parser.Parse(new SequenceInput<char>(inputSeq, true));

        _output.WriteLine(r.ToString());

        var remains = inputSeq.Slice(r.Position);
        Assert.Equal(3, remains.Length);


    }

    [Fact]
    public void Bind()
    {
        var input = "7>>=123abcF".ToCharArray();
        var inputSeq = MemorySegment<char>.ChunkedSequence(input, 2);
        var seqInput = new SequenceInput<char>(inputSeq, true);
        _output.WriteLine(seqInput.ToString());

        var countParser = 
            Parsers.TakeWhile<char>(char.IsDigit)
                .Map(cs => int.Parse(cs.AsSpan()))
                .IgnoreError();
        var delimiterParser = Parsers.Tag(['>', '>', '=']).IgnoreError();
        var sublineParser = (int c) => Parsers.Take<char>(c).IgnoreError();

        var parser = 
            countParser.Bind(c => 
                delimiterParser.Bind(_ => 
                    sublineParser(c).Bind(
                        sc => 
                            Parsers.Eof<char>()
                                .Map(_ => sc)
                        )
                    )
                );

        var r = parser.Parse(seqInput).Map(cs => new string(cs));

        _output.WriteLine($"{r}");

        var remains = inputSeq.Slice(r.Position);
        Assert.Equal(0, remains.Length);
    }
}