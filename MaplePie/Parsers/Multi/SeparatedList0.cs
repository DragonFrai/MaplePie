using MaplePie.Errors;
using MaplePie.Parser;

namespace MaplePie.Parsers.Multi;

public struct SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError>(TParserSep separator, TParser parser)
    : IParser<SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError>, TToken, List<TOutput>, TError>
    where TParserSep : IParser<TParserSep, TToken, TOutputSep, TError>
    where TParser : IParser<TParser, TToken, TOutput, TError>

{
    private TParserSep _separator = separator;
    private TParser _parser = parser;
    
    public static ParseResult<TToken, List<TOutput>, TError> Parse(
        ref SeparatedList0<TParserSep, TParser, TToken, TOutputSep, TOutput, TError> self, 
        ReadOnlySpan<TToken> input, 
        int position)
    {
        var list = new List<TOutput>();

        while (true)
        {
            var parseResult = TParser.Parse(ref self._parser, input, position);
            var parseKind = parseResult.Kind;
            if (parseKind == ParseResultKind.Miss)
            {
                return ParseResult<TToken, List<TOutput>, TError>.Ok(parseResult.Position, list);
            }
            if (parseKind == ParseResultKind.Fail)
            {
                return ParseResult<TToken, List<TOutput>, TError>.Fail(parseResult.Position, parseResult.Error);
            }
            if (parseKind == ParseResultKind.Ok)
            {
                position = parseResult.Position;
                list.Add(parseResult.Output);
            }

            var parseSepResult = TParserSep.Parse(ref self._separator, input, position);
            var parseSepKind = parseSepResult.Kind;
            if (parseSepKind == ParseResultKind.Miss)
            {
                return ParseResult<TToken, List<TOutput>, TError>.Ok(parseSepResult.Position, list);
            }
            if (parseSepKind == ParseResultKind.Fail)
            {
                return ParseResult<TToken, List<TOutput>, TError>.Fail(parseSepResult.Position, parseSepResult.Error);
            }
            if (parseSepKind == ParseResultKind.Ok)
            {
                if (parseSepResult.Position <= position)
                {
                    throw new ParserNotAdvancedException("Separator not advanced.");
                }
                
                position = parseSepResult.Position;
            }
        }


    }
}