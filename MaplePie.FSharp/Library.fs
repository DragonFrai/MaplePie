namespace MaplePie.FSharp

open System
open MaplePie
open MaplePie.Parsers.Combine
open MaplePie.TokenParsers
open MaplePie.BranchParsers
open MaplePie.Errors
open MaplePie.Parser

type JsonValue =
    | Null
    | Bool of bool
    | String of string

module Say =
    let hello name =
        let nullParser = Parse.Tag("null");

        let boolParser = Parse.Alt(
            Parse.Tag("true").Map(fun _ -> true), 
            Parse.Tag("false").Map(fun _ -> false)
        )
        
        let takeStringChar =
            Parse.Tag("\\\"").Map(fun _ -> ())
                .Or(Parse.Tag("\\n").Map(fun _ -> ()))
                .Or(Parse.Tag("\\\\").Map(fun _ -> ()))
                .Or(Parse.TokenIs(fun c -> c <> '"').Map(fun _ -> ()));

        let escapedString =
                Parse.Tag("\"")
                    .Bind(fun _ -> Parse.TakeWhileP(takeStringChar))
                    .EndsWith(Parse.Tag("\""))
                    .MapI(fun span -> span.ToString());

        let valueParser =
            nullParser.Map(fun _ -> JsonValue.Null)
                .Or(boolParser.Map(fun v -> JsonValue.Bool v))
                .Or(escapedString.Map(fun v -> JsonValue.String v))
                .EndsWith(Parse.Eof<char>());
        
        printfn "Hello %s" name