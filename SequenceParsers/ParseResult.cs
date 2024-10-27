using System.Buffers;
using System.Diagnostics;

namespace SequenceParsers;


public enum ParseResultKind
{
    // Successful parsed. Contains count of used data
    Ok,
    
    // Needs more data for parsing
    Need,
    
    // Recoverable error on parsing (other parser can try again)
    Miss,
    
    // Unrecoverable error on parsing
    Fail,
}

public struct ParseResult<TToken, TOutput, TError>
{
    private ParseResultKind _kind;
    private SequencePosition _position;
    private TOutput _output;
    private TError _error;
    private Needed _needed;

    private ParseResult(ParseResultKind kind, SequencePosition position, TOutput output, TError error, Needed needed)
    {
        _kind = kind;
        _position = position;
        _output = output;
        _needed = needed;
        _error = error;
    }

    public static ParseResult<TToken, TOutput, TError> Ok(SequencePosition position, TOutput output)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Ok, position, output, default!, default);
    }
    
    public static ParseResult<TToken, TOutput, TError> Need(SequencePosition position, Needed needed)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Need, position, default!, default!, needed);
    }
    
    public static ParseResult<TToken, TOutput, TError> Miss(SequencePosition position, TError error)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Miss, position, default!, error, default);
    }
    
    public static ParseResult<TToken, TOutput, TError> Fail(SequencePosition position, TError error)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Fail, position, default!, error, default);
    }

    public ParseResultKind Kind => _kind;
    public bool IsOk => _kind == ParseResultKind.Ok;
    public bool IsNeed => _kind == ParseResultKind.Need;
    public bool IsMiss => _kind == ParseResultKind.Miss;
    public bool IsFail => _kind == ParseResultKind.Fail;

    /// <summary>
    /// Parser state position
    /// Parser must be called repeatedly with sequence beginning from this position.
    /// If result error (Miss or Fail), this position can indicate error position.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public SequencePosition Position
    {
        get
        {
            if (_kind != ParseResultKind.Ok && 
                _kind != ParseResultKind.Need && 
                _kind != ParseResultKind.Miss && 
                _kind != ParseResultKind.Fail)
            {
                throw new InvalidOperationException("Get position from not expected result case");
            }

            return _position;
        }
    }
    
    public TOutput Output
    {
        get
        {
            if (_kind != ParseResultKind.Ok)
            {
                throw new InvalidOperationException("Get output from not Ok result");
            }

            return _output;
        }
    }

    public Needed Needed
    {
        get
        {
            if (_kind != ParseResultKind.Need)
            {
                throw new InvalidOperationException("Get needed from not Incomplete result");
            }

            return _needed;
        }
    }

    public TError Error
    {
        get
        {
            if (_kind != ParseResultKind.Miss && _kind != ParseResultKind.Fail)
            {
                throw new InvalidOperationException("Get error from not Error or Failed result");
            }

            return _error;
        }
    }

    public override string ToString()
    {
        return Kind switch
        {
            ParseResultKind.Ok => $"{nameof(ParseResultKind.Ok)}({Output?.ToString()})",
            ParseResultKind.Need => $"{nameof(ParseResultKind.Need)}({Needed.ToString()})",
            ParseResultKind.Miss => $"{nameof(ParseResultKind.Miss)}({Error?.ToString()})",
            ParseResultKind.Fail => $"{nameof(ParseResultKind.Fail)}({Error?.ToString()})",
            _ => throw new UnreachableException()
        };
    }
}

public static class ResultExtensions
{
    public static ParseResult<TToken, TOutput2, TError> Map<TToken, TOutput1, TOutput2, TError>(
        this ParseResult<TToken, TOutput1, TError> parseResult, Func<TOutput1, TOutput2> mapper)
    {
        return parseResult.Kind switch
        {
            ParseResultKind.Ok => ParseResult<TToken, TOutput2, TError>.Ok(parseResult.Position, mapper.Invoke(parseResult.Output)),
            ParseResultKind.Need => ParseResult<TToken, TOutput2, TError>.Need(parseResult.Position, parseResult.Needed),
            ParseResultKind.Miss => ParseResult<TToken, TOutput2, TError>.Miss(parseResult.Position, parseResult.Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutput2, TError>.Fail(parseResult.Position, parseResult.Error),
            _ => throw new UnreachableException()
        };
    }

    public static ParseResult<TToken, TOutput, TError2> MapError<TToken, TOutput, TError1, TError2>(
        this ParseResult<TToken, TOutput, TError1> parseResult, Func<TError1, TError2> mapper)
    {
        return parseResult.Kind switch
        {
            ParseResultKind.Ok => ParseResult<TToken, TOutput, TError2>.Ok(parseResult.Position, parseResult.Output),
            ParseResultKind.Need => ParseResult<TToken, TOutput, TError2>.Need(parseResult.Position, parseResult.Needed),
            ParseResultKind.Miss => ParseResult<TToken, TOutput, TError2>.Miss(parseResult.Position, mapper.Invoke(parseResult.Error)),
            ParseResultKind.Fail => ParseResult<TToken, TOutput, TError2>.Fail(parseResult.Position, mapper.Invoke(parseResult.Error)),
            _ => throw new UnreachableException()
        };
    }
}
