namespace MaplePie;


// /// <summary>
// /// Interface-based parser interface.
// /// </summary>
// public interface IBoxedParser<TToken, TOutVal, TError>
// {
//     public ParseResult<TToken, TOutVal, TError> Parse(SequenceInput<TToken> input);
//     public void Reset();
// }
//
// public class BoxedParser<TParser, TToken, TOutVal, TError>(TParser parser)
//     : IBoxedParser<TToken, TOutVal, TError>, IParser<BoxedParser<TParser, TToken, TOutVal, TError>, TToken, TOutVal, TError>
//     where TParser: struct, IParser<TParser, TToken, TOutVal, TError>
// {
//     public ParseResult<TToken, TOutVal, TError> Parse(ref TParser parser, SequenceInput<TToken> input)
//     {
//         return TParser.Parse(ref parser, input);
//     }
//
//     public void Reset()
//     {
//         TParser.Reset(ref parser);
//     }
//
//     public static ParseResult<TToken, TOutVal, TError> Parse(ref BoxedParser<TParser, TToken, TOutVal, TError> self, SequenceInput<TToken> input)
//     {
//         return self.Parse(input);
//     }
//
//     public static void Reset(ref BoxedParser<TParser, TToken, TOutVal, TError> self)
//     {
//         self.Reset();
//     }
// }
//
// public struct BoxedParserRef<TToken, TOutVal, TError>: IParser<BoxedParserRef<TToken, TOutVal, TError>, TToken, TOutVal, TError>
// {
//     private IBoxedParser<TToken, TOutVal, TError> _boxedParser;
//
//     public BoxedParserRef(IBoxedParser<TToken, TOutVal, TError> boxedParser)
//     {
//         _boxedParser = boxedParser;
//     }
//
//     public IBoxedParser<TToken, TOutVal, TError> Inner => _boxedParser;
//     
//     public static ParseResult<TToken, TOutVal, TError> Parse(ref BoxedParserRef<TToken, TOutVal, TError> self, SequenceInput<TToken> input)
//     {
//         return self._boxedParser.Parse(input);
//     }
//
//     public static void Reset(ref BoxedParserRef<TToken, TOutVal, TError> self)
//     {
//         self._boxedParser.Reset();
//     }
// }
//
// public static class FundamentalBoxedParserExtensions
// {
//     public static IBoxedParser<TToken, TOutVal, TError> ToBoxed<TParser, TToken, TOutVal, TError>(
//         this Parser<TParser, TToken, TOutVal, TError> parser)
//         where TParser : struct, IParser<TParser, TToken, TOutVal, TError>
//     {
//         return new BoxedParser<TParser, TToken, TOutVal, TError>(parser.Inner);
//     }
//     
//     public static Parser<BoxedParserRef<TToken, TOutVal, TError>, TToken, TOutVal, TError> ToParser<TToken, TOutVal, TError>(
//         this IBoxedParser<TToken, TOutVal, TError> boxedParser)
//     {
//         return new Parser<BoxedParserRef<TToken, TOutVal, TError>, TToken, TOutVal, TError>(
//             new BoxedParserRef<TToken, TOutVal, TError>(boxedParser));
//     }
//     
//     public static Parser<BoxedParserRef<TToken, TOutVal, TError>, TToken, TOutVal, TError> ToBoxedRef<TParser, TToken, TOutVal, TError>(
//         this Parser<TParser, TToken, TOutVal, TError> parser)
//         where TParser : struct, IParser<TParser, TToken, TOutVal, TError>
//     {
//         return parser.ToBoxed().ToParser();
//     }
// }
