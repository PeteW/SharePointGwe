// This project type requires the F# PowerPack at http://fsharppowerpack.codeplex.com/releases

(*
*)
module GweParser
//namespace Gwe.Lang


open System
open Microsoft.FSharp.Text.Lexing
open Oxbow.Gwe.Lang.Ast
open Lexer
open Parser

let eval input =
    let lexbuff = LexBuffer<char>.FromString(input)
    let equation = Parser.start Lexer.tokenize lexbuff
    equation

/// Evaluate an expression
let rec evalExpr expr =
    match expr with
    //| x :: y -> evalExpr x + evalExpr y
    | Func(x,y) -> x + "((" + List.fold( fun acc x -> acc + " -="+evalExpr(x)+"=- ") "" y + "))"
    | Text x -> x

/// Evaluate an equation
and evalEquation eq =
    match eq with
    | Equation expr -> evalExpr expr

let rec readAndProcess() =
    printf ":"
    match Console.ReadLine() with
    | "quit" -> ()
    | expr ->
        try
            printfn "Lexing [%s]" expr
            let lexbuff = LexBuffer<char>.FromString(expr)
            
            printfn "Parsing..."
            let equation = Parser.start Lexer.tokenize lexbuff
            
            printfn "Evaluating Equation..."
            let result = evalEquation equation
            
            printfn "Result: %s" (result.ToString())
            
        with ex ->
            printfn "Unhandled Exception: %s" ex.Message

        readAndProcess()

readAndProcess()


