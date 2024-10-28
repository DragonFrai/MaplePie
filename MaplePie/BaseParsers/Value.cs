using MaplePie.Utils;

namespace MaplePie.BaseParsers;

//
// /// <summary>
// /// Returns value without consuming input.
// /// </summary>
// /// <typeparam name="TToken"></typeparam>
// /// <typeparam name="TOutVal"></typeparam>
// public struct ValueParser<TToken, TOutVal> : IParser<ValueParser<TToken, TOutVal>, TToken, TOutVal, Never>
// {
//     private ValueSource<TOutVal> _output;
//
//     public ValueParser(ValueSource<TOutVal> output)
//     {
//         _output = output;
//     }
//     
//     public static ParseResult<TToken, TOutVal, Never> Parse(ref ValueParser<TToken, TOutVal> self, SequenceInput<TToken> input)
//     {
//         return ParseResult<TToken, TOutVal, Never>.Ok(input.Start, self._output.Get());
//     }
//
//     public static void Reset(ref ValueParser<TToken, TOutVal> self)
//     { }
// }
