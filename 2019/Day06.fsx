#load "utils/Base.fsx"
open Base

let orbits =
    inputFile "06" (ssplit ")")

let toTuples arr =
    match arr with
    | [|a; b|] -> b,a
    | err -> failwithf "Invalid orbit: %A" err

let solve (orbits:seq<string[]>) =
    let tups = orbits |> Seq.map toTuples
    let map = tups |> Map.ofSeq

    let rec getCenter c depth =
        match c with
        | "COM" -> depth
        | _ -> getCenter map.[c] (depth+1)

    tups
    |> Seq.map (fun (r,c) -> r,(getCenter c 1))
    |> Seq.sumBy snd

inputFile "06test1" (ssplit ")")
|> solve |> solution "06a.1" 42

inputFile "06" (ssplit ")")
|> solve |> solution "06a" 147223

let solveB (orbits:seq<string[]>) =
    let tups = orbits |> Seq.map toTuples
    let map = tups |> Map.ofSeq

    let rec getCenter path =
        match path with
        | "COM"::_ -> path |> Set.ofList
        | head::_ -> getCenter (map.[head]::path)
        | err -> failwithf "Invalid path: %A" err

    let you,san =
        map
        |> Map.map (fun _ c -> getCenter [c])
        |> fun m -> m.["YOU"],m.["SAN"]
    
    (Set.difference you san |> Set.count) + (Set.difference san you |> Set.count)

inputFile "06test2" (ssplit ")")
|> solveB |> solution "06a.1" 4

inputFile "06" (ssplit ")")
|> solveB |> solution "06b" 340