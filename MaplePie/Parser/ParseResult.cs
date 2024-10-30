using System.Diagnostics;

namespace MaplePie.Parser;


public enum ParseResultKind
{
    // Successful parsed
    Ok,
    
    // Recoverable error on parsing (other parser can try again)
    Miss,
    
    // Unrecoverable error on parsing
    Fail,
}

public struct ParseResult<TToken, TOutput, TError>
{
    private ParseResultKind _kind;
    private int _position;
    private TOutput _output;
    private TError _error;

    private ParseResult(ParseResultKind kind, int position, TOutput output, TError error)
    {
        _kind = kind;
        _position = position;
        _output = output;
        _error = error;
    }

    public static ParseResult<TToken, TOutput, TError> Ok(int position, TOutput output)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Ok, position, output, default!);
    }

    public static ParseResult<TToken, TOutput, TError> Miss(int position, TError error)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Miss, position, default!, error);
    }
    
    public static ParseResult<TToken, TOutput, TError> Fail(int position, TError error)
    {
        return new ParseResult<TToken, TOutput, TError>(ParseResultKind.Fail, position, default!, error);
    }

    public ParseResultKind Kind => _kind;
    public bool IsOk => _kind == ParseResultKind.Ok;
    public bool IsMiss => _kind == ParseResultKind.Miss;
    public bool IsFail => _kind == ParseResultKind.Fail;

    public ParseResult<TToken, TOutputX, TError> Anything<TOutputX>()
    {
        return Kind switch
        {
            ParseResultKind.Miss => ParseResult<TToken, TOutputX, TError>.Miss(Position, Error),
            ParseResultKind.Fail => ParseResult<TToken, TOutputX, TError>.Fail(Position, Error),
            ParseResultKind.Ok => throw new InvalidOperationException($"{nameof(Anything)} called for not error result."),
            _ => throw new UnreachableException()
        };
    }
    
    /// <summary>
    /// Not parsed state for executed parser.
    /// Used for passing in next parser in Ok case,
    /// and for approx error location in Miss and Fail cases.
    /// In Miss and Fail cases should be filled last known input reminder or empty range,
    /// but it must always be valid input sequence range (no out of range of input).
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public int Position
    {
        get
        {
            if (_kind != ParseResultKind.Ok && _kind != ParseResultKind.Miss && _kind != ParseResultKind.Fail)
            {
                throw new UnreachableException();
                // throw new InvalidOperationException("Get remainder from not expected result case");
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

    public ReadOnlySpan<TToken> Remainder(ReadOnlySpan<TToken> input)
    {
        return input.Slice(Position);
    }

    public override string ToString()
    {
        return Kind switch
        {
            ParseResultKind.Ok => $"{nameof(ParseResultKind.Ok)}({Output?.ToString()})",
            ParseResultKind.Miss => $"{nameof(ParseResultKind.Miss)}({Error?.ToString()})",
            ParseResultKind.Fail => $"{nameof(ParseResultKind.Fail)}({Error?.ToString()})",
            _ => throw new UnreachableException()
        };
    }
    
    public string ToString(ReadOnlySpan<TToken> input)
    {
        var position = Position;
        var limitedLength = Math.Min(input.Length - position, 16);
        var reminder = input.Slice(position, limitedLength);
        return Kind switch
        {
            ParseResultKind.Ok => $"{nameof(ParseResultKind.Ok)}({Output?.ToString()}, rem='{reminder.ToString()}')",
            ParseResultKind.Miss => $"{nameof(ParseResultKind.Miss)}({Error?.ToString()}, rem='{reminder.ToString()}')",
            ParseResultKind.Fail => $"{nameof(ParseResultKind.Fail)}({Error?.ToString()}, rem='{reminder.ToString()}')",
            _ => throw new UnreachableException()
        };
    }
}

public static class ResultExtensions
{
    public static 
        ReadOnlySpan<TToken> 
        InputOutput<TToken, TError>(
            this ParseResult<TToken, InputRange, TError> result,
            ReadOnlySpan<TToken> input)
    {
        return input.Slice(result.Output.Position, result.Output.Length);
    }
    
    // public static ParseResult<TToken, TOutVal2, TError> Map<TToken, TOutVal1, TOutVal2, TError>(
    //     this ParseResult<TToken, TOutVal1, TError> parseResult, Func<TOutVal1, TOutVal2> mapper)
    // {
    //     return parseResult.Kind switch
    //     {
    //         ParseResultKind.Ok => ParseResult<TToken, TOutVal2, TError>.Ok(parseResult.remains, mapper.Invoke(parseResult.Output)),
    //         ParseResultKind.Need => ParseResult<TToken, TOutVal2, TError>.Need(parseResult.remains, parseResult.Needed),
    //         ParseResultKind.Miss => ParseResult<TToken, TOutVal2, TError>.Miss(parseResult.remains, parseResult.Error),
    //         ParseResultKind.Fail => ParseResult<TToken, TOutVal2, TError>.Fail(parseResult.remains, parseResult.Error),
    //         _ => throw new UnreachableException()
    //     };
    // }
    //
    // public static ParseResult<TToken, TOutVal, TError2> MapError<TToken, TOutVal, TError1, TError2>(
    //     this ParseResult<TToken, TOutVal, TError1> parseResult, Func<TError1, TError2> mapper)
    // {
    //     return parseResult.Kind switch
    //     {
    //         ParseResultKind.Ok => ParseResult<TToken, TOutVal, TError2>.Ok(parseResult.remains, parseResult.Output),
    //         ParseResultKind.Need => ParseResult<TToken, TOutVal, TError2>.Need(parseResult.remains, parseResult.Needed),
    //         ParseResultKind.Miss => ParseResult<TToken, TOutVal, TError2>.Miss(parseResult.remains, mapper.Invoke(parseResult.Error)),
    //         ParseResultKind.Fail => ParseResult<TToken, TOutVal, TError2>.Fail(parseResult.remains, mapper.Invoke(parseResult.Error)),
    //         _ => throw new UnreachableException()
    //     };
    // }
}
