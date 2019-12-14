#load "utils/Base.fsx"
open Base
#load "utils/ICC.fsx"
open ICC

let program =
    inputFile "05" string
    |> Seq.exactlyOne
    |> ssplit ","
    |> Array.map int

let run input program =
    program
    |> Array.map int64
    |> IntcodeComputer.run input 
    |> List.head
    |> int

run [7] [|3;9;8;9;10;9;4;9;99;-1;8|]
|> solution "05b.1a" 0
run [8] [|3;9;8;9;10;9;4;9;99;-1;8|]
|> solution "05b.1b" 1

run [7] [|3;9;7;9;10;9;4;9;99;-1;8|]
|> solution "05b.2a" 1
run [8] [|3;9;7;9;10;9;4;9;99;-1;8|]
|> solution "05b.2b" 0

run [7] [|3;3;1108;-1;8;3;4;3;99|]
|> solution "05b.3a" 0
run [8] [|3;3;1108;-1;8;3;4;3;99|]
|> solution "05b.3b" 1

run [7] [|3;3;1107;-1;8;3;4;3;99|]
|> solution "05b.4a" 1
run [8] [|3;3;1107;-1;8;3;4;3;99|]
|> solution "05b.4b" 0

run [0] [|3;12;6;12;15;1;13;14;13;4;13;99;-1;0;1;9|]
|> solution "05b.5a" 0
run [1] [|3;12;6;12;15;1;13;14;13;4;13;99;-1;0;1;9|]
|> solution "05b.5b" 1

run [0] [|3;3;1105;-1;9;1101;0;0;12;4;12;99;1|]
|> solution "05b.6a" 0
run [1] [|3;3;1105;-1;9;1101;0;0;12;4;12;99;1|]
|> solution "05b.6b" 1

run [7] [|3;21;1008;21;8;20;1005;20;22;107;8;21;20;1006;20;31;1106;0;36;98;0;0;1002;21;125;20;4;20;1105;1;46;104;999;1105;1;46;1101;1000;1;20;4;20;1105;1;46;98;99|]
|> solution "05b.7a" 999
run [8] [|3;21;1008;21;8;20;1005;20;22;107;8;21;20;1006;20;31;1106;0;36;98;0;0;1002;21;125;20;4;20;1105;1;46;104;999;1105;1;46;1101;1000;1;20;4;20;1105;1;46;98;99|]
|> solution "05b.7b" 1000
run [9] [|3;21;1008;21;8;20;1005;20;22;107;8;21;20;1006;20;31;1106;0;36;98;0;0;1002;21;125;20;4;20;1105;1;46;104;999;1105;1;46;1101;1000;1;20;4;20;1105;1;46;98;99|]
|> solution "05b.7c" 1001

run [5] program
|> solution "05b" 12648139
run [1] program
|> solution "05a" 4511442

