namespace Oxbow.Gwe.Lang.Ast
open System

type Expr =
    | Func of string * Expr list
    | Text of string    

type Equation =
    | Equation of Expr
