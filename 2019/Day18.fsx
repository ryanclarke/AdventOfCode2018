#load "utils/Base.fsx"
open Base

type Cell = {
    Dist: int
    Path: int list
    Coord: int * int
    Done: bool
    Code: int
}

type Path = {
    Dist: int
    Objects: int list
}

let run file =
    let vault = 
        inputFile file string
        |> Seq.map (Seq.toArray >> Array.map int)
        |> Array.ofSeq

    let width = vault.[0].Length-1
    let height = vault.Length-1

    let origin = 64
    let wall = 35
    let space = 46
    let isKey i = i >= 97 && i <= 122
    let isKeyOrOrigin i = i >= 97 && i <= 122 || i = origin
    let isDoor i = isKey (i + 32)
    let isObject i = isKey i || isDoor i
    let isKeyForDoor k d = isKey k && k - d = 32
    let doorAndKey k = if isKey k then [k; k - 32] else []

    let defaultAllowed = [origin;space]
    let asChar is = is |> Array.map char
    let asInt cs = cs |> Array.map int

    let cardsFrom (y,x) = [
        if x > 0 then yield (y,x-1)
        if x < width then yield (y,x+1)
        if y > 0 then yield (y+1,x)
        if y < height then yield (y-1,x)
    ]

    let isGood c = (not c.Done) && c.Code <> wall

    let dumprIfEmpty (a:'a list) = if a.Length = 0 then dumpr a else a

    let minDist (cells:Cell []) =
        cells |> Array.filter isGood |> Array.minBy (fun c -> c.Dist)

    let ys = [| 0 .. height |]
    let xs = [| 0 .. width |]
    let neighborMap = ys |> Array.collect (fun y -> xs |> Array.map (fun x -> ((y,x),(cardsFrom (y,x))))) |> Map.ofArray
    let locationMap =
        vault
        |> Array.mapi (fun y ys -> ys |> Array.mapi (fun x xChar -> (xChar, (y,x))))
        |> Array.collect id
        |> Array.groupBy fst 
        |> Array.filter (fun (_,cs) -> cs.Length = 1) 
        |> Array.map (fun (c,cs) -> c,snd cs.[0])
        |> Map.ofArray

    let dumpScreen tick (minX,maxX,minY,maxY) (cells:Cell [] []) =
        [minY .. maxY] |> List.iter (fun y ->
            [minX .. maxX] |> List.map (fun x ->
                char cells.[y].[x].Code)
            |> dumpcs
        )
        printfn "======= #%A %A" tick (minX,maxX,minY,maxY)

    let dumpFullscreen = dumpScreen 0 (0,width,0,height)

    let distance start target =
        let mutable cells = ys |> Array.collect (fun y -> xs |> Array.map (fun x -> {Dist = System.Int32.MaxValue; Path = []; Coord = (y,x); Done = false; Code = vault.[y].[x]}))
        let startY,startX = locationMap.[start]
        let idx y x = y * (width+1) + x  

        // dumpFullscreen cells
        
        cells.[idx startY startX] <- {cells.[idx startY startX] with Dist = 0}

        let mutable minX = startX
        let mutable maxX = startX
        let mutable minY = startY
        let mutable maxY = startY
        // dump ([|[|cells.[startY].[startX]|]|] |> minDist allowedChars)

        let isAvailable (y,x) = cells.[idx y x] |> isGood
        let mutable targetCell = None
        let mutable tick = 0

        let mutable currentCell = minDist cells
        while (targetCell.IsNone && currentCell.Dist < 100000) do
            let possibilities = neighborMap.[currentCell.Coord]

            possibilities
            |> List.iter (fun (y,x) ->
                let cell = cells.[idx y x]
                if isAvailable (y,x)
                then
                    let path = currentCell.Code::currentCell.Path
                    if path.Length < cell.Dist
                    then
                        if x < minX then minX <- x
                        if x > maxX then maxX <- x
                        if y < minY then minY <- y
                        if y > maxY then maxY <- y
                        cells.[idx y x] <- {cell with Dist = path.Length; Path = path}
                    if cell.Code = target
                    then targetCell <- Some cells.[idx y x]
                else 
                    cells.[idx y x] <- {cell with Done = true}
            )
        
            // dumpScreen tick (minX,maxX,minY,maxY) cells |> ignore

            let myY,myX = currentCell.Coord
            cells.[idx myY myX] <- {currentCell with Done = true}
            tick <- tick + 1
            currentCell <- minDist cells
        
        // dumpScreen tick (minX,maxX,minY,maxY) cells
        let tc = targetCell.Value 
        {Dist = tc.Dist; Objects = (tc.Path |> List.distinct |> List.rev |> List.tail |> List.filter isObject)}

    let vaultKeys = vault |> Array.collect id |> Array.toList |> List.groupBy id |> List.map (fst >> int) |> List.filter (isKey) |> List.sort
    let paths = 
        vaultKeys |> List.append [origin] |> List.collect (fun a ->
            vaultKeys |> List.choose (fun b ->
                if a >= b
                then None
                else
                    let path = distance (int a) (int b)
                    Some [(a,b),path; (b,a),path]
            ) |> List.collect id
        )
        |> Map.ofList

    // ===================================================================================================================
    // 2019.18 A
    // ===================================================================================================================
    

    let mutable minimumTraveled = System.Int32.MaxValue
    let rec findem depth (traveled:int) (visited:int list) (objects:int list) (start:int) (targets:int list) : int =
        //dumps "======================" depth
        if depth < 3 then dumptss (sprintf "%3i %A" traveled (visited |> List.map char |> List.rev))
        match (targets,traveled) with
        | [],_ -> 
                if traveled < minimumTraveled
                then
                    minimumTraveled <- traveled
                    dumptss (sprintf "%3i %A" traveled (visited |> List.map char |> List.rev))
                traveled
        | _,t when t >= minimumTraveled -> System.Int32.MaxValue
        | _,_ ->
            targets
            |> List.map (fun target -> target,paths.[(start,target)])
            // |> dumprs "Targets"
            |> List.filter (fun (_,path) -> path.Objects |> List.forall (fun obj -> objects |> List.contains obj))
            // |> dumprs "Filtered"
            |> List.sortBy (fun (_,path) -> path.Dist)
            // |> (fun c -> printf "%i " c.Length; c)
            |> List.map (fun (target,path) -> 
                // dumps "path" p
                let newTraveled = (traveled + path.Dist)
                let newVisited = target::visited
                let newObjects = List.append (doorAndKey target) objects // |> dumprs "New Objects"
                let newTargets = List.except [target] targets // |> dumprs "New Targets"
                findem (depth+1) newTraveled newVisited newObjects target newTargets
            ) |> List.min

    findem 0 0 [] [] origin vaultKeys


run "18test.1"
|> solution "18a.1" 86
run "18test.2"
|> solution "18a.2" 132
// run "18test.3"
// |> solution "18a.3" 136
run "18test.4"
|> solution "18a.4" 81
// run "18"
// |> solution "18a" 6162

// ===================================================================================================================
// 2019.18 B
// ===================================================================================================================

// let solveB _ = 0

// solveB vault
// |> solution "18b" 0