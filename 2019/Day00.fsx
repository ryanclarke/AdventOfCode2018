#load "utils/Base.fsx"
open Base
#load "utils/ICC.fsx"
open ICC

let initial =
    inputFile "00" string
    |> Seq.exactlyOne
    |> ssplit ","
    |> Array.map int

// ===================================================================================================================
// 2019.00 A
// ===================================================================================================================

let solveA _ = 0

solveA initial
|> solution "00a" 0

// ===================================================================================================================
// 2019.00 B
// ===================================================================================================================

let solveB _ = 0

solveB initial
|> solution "00b" 0