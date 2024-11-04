using System.Diagnostics;
using MaplePie.Errors;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.Parsers.Multi;




public static class FoldParser
{
    public static 
        ParseResult<TToken, TState, TError> 
        Parse<TParser, TToken, TOutput, TState, TError> 
        (
            ref TParser parser,
            Func<TState> initializer,
            Func<TState, TOutput, TState> folder,
            int minimum,
            int maximum,
            ValueSource<TError> minimumUnsatisfiedErrorSource,
            ReadOnlySpan<TToken> input,
            int position
        )
        where TParser : IParser<TParser, TToken, TOutput, TError>
    {
        var count = 0;
        var currentPosition = position;
        var state = initializer();
        while (true)
        {
            var result = TParser.Parse(ref parser, input, currentPosition);
            switch (result.Kind)
            {
                case ParseResultKind.Ok:
                {
                    if (result.Position <= currentPosition)
                    {
                        throw new ParserNotAdvancedException();
                    }

                    count += 1;

                    if (maximum != -1 && count >= maximum)
                    {
                        return ParseResult<TToken, TState, TError>.Ok(result.Position, state);
                    }
                    
                    currentPosition = result.Position;
                    state = folder(state, result.Output);
                    break;
                }
                case ParseResultKind.Miss:
                {
                    if (minimum != -1 && count < minimum)
                    {
                        return ParseResult<TToken, TState, TError>.Miss(currentPosition, minimumUnsatisfiedErrorSource.Get());
                    }
                    return ParseResult<TToken, TState, TError>.Ok(currentPosition, state);
                }
                case ParseResultKind.Fail:
                {
                    return ParseResult<TToken, TState, TError>.Fail(result.Position, result.Error);
                }
                default:
                    throw new UnreachableException();
            }
        }
    }
}

public struct 
    FoldParser<TParser, TToken, TOutput, TState, TError>:
    IParser<FoldParser<TParser, TToken, TOutput, TState, TError>, TToken, TState, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>
{
    private TParser _parser;
    private Func<TState> _init;
    // Note: Folder is function with return so that state can fully replaced
    // (useful for struct states or static class instances.)
    private Func<TState, TOutput, TState> _folder;

    // Minimal inner parser iterations count
    private int _minimum;
    // Maximum inner parser iterations count
    private int _maximum;

    // private ValueSource<TError> _parserNotProgressedErrorSource;
    private ValueSource<TError> _minimumUnsatisfiedErrorSource;

    public FoldParser(
        TParser parser, 
        Func<TState> init, 
        Func<TState, TOutput, TState> folder,
        int minimum,
        int maximum,
        ValueSource<TError> minimumUnsatisfiedErrorSource)
    {
        _parser = parser;
        _init = init;
        _folder = folder;
        _minimum = minimum;
        _maximum = maximum;
        _minimumUnsatisfiedErrorSource = minimumUnsatisfiedErrorSource;
    }
    
    public FoldParser(
        TParser parser, 
        Func<TState> init, 
        Func<TState, TOutput, TState> folder,
        int minimum,
        ValueSource<TError> minimumUnsatisfiedErrorSource)
    {
        _parser = parser;
        _init = init;
        _folder = folder;
        _minimum = minimum;
        _maximum = -1;
        _minimumUnsatisfiedErrorSource = minimumUnsatisfiedErrorSource;
    }

    public FoldParser(
        TParser parser, 
        Func<TState> init, 
        Func<TState, TOutput, TState> folder
        )
    {
        _parser = parser;
        _init = init;
        _folder = folder;
        _minimum = -1;
        _maximum = -1;
        _minimumUnsatisfiedErrorSource = ValueSource<TError>.Func(() => throw new UnreachableException());
    }
    
    public static 
        ParseResult<TToken, TState, TError> 
        Parse
        (
            ref FoldParser<TParser, TToken, TOutput, TState, TError> self, 
            ReadOnlySpan<TToken> input, 
            int position
        )
    {
        return 
            FoldParser.Parse
            (
                ref self._parser, 
                self._init, 
                self._folder, 
                self._minimum, 
                self._maximum,
                self._minimumUnsatisfiedErrorSource, 
                input, 
                position
            );
    }
}