using System.Buffers;
using System.Diagnostics;
using MaplePie.Utils;

namespace MaplePie.Branches;

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
