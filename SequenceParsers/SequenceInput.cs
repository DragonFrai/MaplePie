using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace SequenceParsers;

public readonly struct SequenceInput<TToken>
{
    private readonly ReadOnlySequence<TToken> _sequence;
    private readonly bool _isCompleted;

    public SequenceInput(ReadOnlySequence<TToken> sequence, bool isCompleted)
    {
        _sequence = sequence;
        _isCompleted = isCompleted;
    }

    public ReadOnlySequence<TToken> AsSequence => _sequence;
    public bool IsCompleted => _isCompleted;
    
    public ReadOnlyMemory<TToken> First => _sequence.First;
    public ReadOnlySpan<TToken> FirstSpan => _sequence.FirstSpan;
    public bool IsSingleSegment => _sequence.IsSingleSegment;
    public bool IsEmpty => _sequence.IsEmpty;
    public SequencePosition Start => _sequence.Start;
    public SequencePosition End => _sequence.End;
    public long Length => _sequence.Length;
    
    public long GetOffset(SequencePosition position) => _sequence.GetOffset(position);
    
    public SequencePosition GetPosition(long offset, SequencePosition origin) => _sequence.GetPosition(offset, origin);
    
    public SequencePosition GetPosition(long offset) => _sequence.GetPosition(offset);
    
    public ReadOnlySequence<TToken>.Enumerator GetEnumerator() => _sequence.GetEnumerator();
    
    public bool TryGet(
        ref SequencePosition position, 
        out ReadOnlyMemory<TToken> memory, 
        bool advance = true) 
        => _sequence.TryGet(ref position, out memory, advance);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("SequenceInput<");
        sb.Append(nameof(TToken));
        sb.Append(">(");
        if (_isCompleted)
        {
            sb.Append(_sequence.Length);
            sb.Append(", ");
            sb.Append("END");
        }
        else
        {
            sb.Append(_sequence.Length);
        }
        sb.Append(")[");
        var current = 0;
        foreach (var segment in _sequence)
        {
            foreach (var token in segment.Span)
            {
                if (current < _sequence.Length - 1)
                {
                    sb.Append(token);
                    sb.Append(", ");
                }
                else
                {
                    sb.Append(token);
                }
                current += 1;
            }
        }
        sb.Append(']');
        return sb.ToString();
    }

    public SequenceInput<TToken> Slice(int start) =>
        new(_sequence.Slice(start), _isCompleted);
    
    public SequenceInput<TToken> Slice(int start, int length) =>
        new(_sequence.Slice(start, length), _isCompleted);
    
    public SequenceInput<TToken> Slice(SequencePosition start, SequencePosition end) =>
        new(_sequence.Slice(start, end), _isCompleted);
    
    public SequenceInput<TToken> Slice(SequencePosition start) =>
        new(_sequence.Slice(start), _isCompleted);
}
