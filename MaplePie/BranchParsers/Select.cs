using System.Diagnostics;
using MaplePie.Parser;
using MaplePie.Utils;

namespace MaplePie.BranchParsers;


public struct Select2Parser<TParser1, TParser2, TToken, TOutput, TError> 
    : IParser<Select2Parser<TParser1, TParser2, TToken, TOutput, TError>, TToken, TOutput, TError>
    where TParser1: IParser<TParser1, TToken, TOutput, TError>
    where TParser2: IParser<TParser2, TToken, TOutput, TError>
{
    private TParser1 _parser1;
    private TParser2 _parser2;
    private ValueSource<TError> _notSelectedError;

    public Select2Parser(TParser1 parser1, TParser2 parser2, ValueSource<TError> notSelectedError)
    {
        _parser1 = parser1;
        _parser2 = parser2;
        _notSelectedError = notSelectedError;
    }
    
    public static
        ParseResult<TToken, TOutput, TError> Parse(ref Select2Parser<TParser1, TParser2, TToken, TOutput, TError> self,
            ReadOnlySpan<TToken> input,
            int range)
    {
        var result1 = TParser1.Parse(ref self._parser1, input, range);
        switch (result1.Kind)
        {
            case ParseResultKind.Ok: 
                return ParseResult<TToken, TOutput, TError>.Ok(result1.Position, result1.Output);
            case ParseResultKind.Fail:
                return 
                    ParseResult<TToken, TOutput, TError>.Fail
                    (
                        result1.Position, 
                        result1.Error
                    );
            case ParseResultKind.Miss: break;
            default: throw new UnreachableException();
        }

        var result2 = TParser2.Parse(ref self._parser2, input, range);
        switch (result2.Kind)
        {
            case ParseResultKind.Ok: 
                return ParseResult<TToken, TOutput, TError>.Ok(result2.Position, result2.Output);
            case ParseResultKind.Fail:
                return 
                    ParseResult<TToken, TOutput, TError>.Fail
                    (
                        result2.Position, 
                        result2.Error
                    );
            case ParseResultKind.Miss: break;
            default: throw new UnreachableException();
        }

        return ParseResult<TToken, TOutput, TError>.Miss(range, self._notSelectedError.Get());
    }
}


//
// public class AnyBoxedParser<TToken, TOutVal, TError> : IBoxedParser<TToken, TOutVal, TError>
// {
//     private readonly IBoxedParser<TToken, TOutVal, TError>[] _parsers;
//     
//     private ValueSource<TError> _altNotChosenErrorSource;
//     
//     // -1 if completed
//     private int _current;
//     
//     public AnyBoxedParser(IBoxedParser<TToken, TOutVal, TError>[] parsers, ValueSource<TError> altNotChosenErrorSource)
//     {
//         if (parsers.Length == 0)
//         {
//             throw new ArgumentException("Parsers must be not empty", nameof(parsers));
//         }
//         
//         _parsers = parsers;
//         _current = 0;
//         _altNotChosenErrorSource = altNotChosenErrorSource;
//     }
//
//     private void CompletionAssert()
//     {
//         if (_current == -1)
//         {
//             throw new InvalidOperationException("Re-using parser without reset.");
//         }
//     }
//     
//     public ParseResult<TToken, TOutVal, TError> Parse(SequenceInput<TToken> input)
//     {
//         CompletionAssert();
//         
//         var current = _current;
//         var length = _parsers.Length;
//         while (current < length)
//         {
//             var parser = _parsers[current];
//             var result = parser.Parse(input);
//
//             switch (result.Kind)
//             {
//                 case ParseResultKind.Ok:
//                     _current = -1;
//                     return result;
//                 case ParseResultKind.Fail:
//                     _current = -1;
//                     return result;
//                 case ParseResultKind.Miss:
//                     current += 1;
//                     continue;
//                 case ParseResultKind.Need:
//                     _current = current;
//                     return result;
//                 default: throw new UnreachableException();
//             }
//         }
//         _current = -1;
//         return ParseResult<TToken, TOutVal, TError>.Miss(input.AsSequence.Start, _altNotChosenErrorSource.Get());
//     }
//
//     public void Reset()
//     {
//         foreach (var parser in _parsers)
//         {
//             parser.Reset();
//         }
//
//         _current = 0;
//     }
// }
