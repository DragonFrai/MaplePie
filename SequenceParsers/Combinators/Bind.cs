using System.Buffers;
using System.Diagnostics;

namespace SequenceParsers.Combinators;


// Большой уровень вложенности может приводить к длинной цепочке вызовов,
// что уменьшит производительность и может исчерпать стек.
//
// Оптимизация через уплощение внутренних NextParser'ов не тривиальна,
// т.к. требуется сохранение исходных параметров из-за ресетинга.
// 
// Пока имеет смысл просто ничего не делать.
public class BindParser<TToken, TOutput1, TOutput2, TError>: IParser<TToken, TOutput2, TError>
{
    private int _state;
    private IParser<TToken, TOutput1, TError> _parser1;
    private IParser<TToken, TOutput2, TError>? _parser2;
    private Func<TOutput1, IParser<TToken, TOutput2, TError>> _binder1;

    public BindParser(IParser<TToken, TOutput1, TError> parser,
        Func<TOutput1, IParser<TToken, TOutput2, TError>> binder)
    {
        _state = 1;
        _parser1 = parser;
        _parser2 = null;
        _binder1 = binder;
    }
    
    public ParseResult<TToken, TOutput2, TError> Parse(SequenceInput<TToken> input)
    {
        ParseResult<TToken, TOutput2, TError> result2;
        switch (_state)
        {
            case -1:
                throw new ParserIsCompletedException();
            case 1:
                var result1 = _parser1.Parse(input);
                switch (result1.Kind)
                {
                    case ParseResultKind.Ok:
                        var parser2 = _binder1(result1.Output);
                        result2 = parser2.Parse(input.Slice(result1.Position));
                        switch (result2.Kind)
                        {
                            case ParseResultKind.Ok:
                            case ParseResultKind.Miss:
                            case ParseResultKind.Fail:
                                _state = -1;
                                return result2;
                            case ParseResultKind.Need:
                                _state = 2;
                                return result2;
                            default:
                                throw new UnreachableException();
                        }
                    case ParseResultKind.Need:
                        return ParseResult<TToken, TOutput2, TError>.Need(result1.Position, result1.Needed);
                    case ParseResultKind.Miss:
                    case ParseResultKind.Fail:
                        _state = -1;
                        return result1.Map<TToken, TOutput1, TOutput2, TError>(_ => throw new UnreachableException());
                    default: throw new UnreachableException();
                }
            case 2:
                result2 = _parser2!.Parse(input);
                switch (result2.Kind)
                {
                    case ParseResultKind.Ok:
                    case ParseResultKind.Miss:
                    case ParseResultKind.Fail:
                        _state = -1;
                        return result2;
                    case ParseResultKind.Need:
                        _state = 2;
                        return result2;
                    default:
                        throw new UnreachableException();
                }
            default: throw new UnreachableException();
        }
    }

    public void Reset()
    {
        _state = 1;
        _parser1.Reset();
        if (_parser2 != null)
        {
            _parser2.Reset();
            _parser2 = null;
        }
    }
}