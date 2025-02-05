﻿// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
namespace Assignment_7

    module internal Program = 
        open JParsec.TextParser
        //open FParsecLight.TextParser
        open ImpParser

        let singleLetterScore = Map.add 0 "_result_ := pointValue(_pos_) + _acc_" Map.empty
        let doubleLetterScore = Map.add 0 "_result_ := pointValue(_pos_) * 2 + _acc_" Map.empty
        let tripleLetterScore = Map.add 0 "_result_ := pointValue(_pos_) * 3 + _acc_" Map.empty

        let doubleWordScore = Map.add 1 "_result_ := _acc_ * 2" singleLetterScore
        let tripleWordScore = Map.add 1 "_result_ := _acc_ * 3" singleLetterScore

        let standardBoardSource = 
          "declare xabs;
           declare yabs;
           if (_x_ < 0) then { xabs := _x_ * -1 } else { xabs := _x_ };
           if (_y_ < 0) then { yabs := _y_ * -1 } else { yabs := _y_ };
           if ((xabs = 0 /\ (yabs = 7)) \/
               (xabs = 7 /\ (yabs = 0 \/ yabs = 7))) then { _result_ := 4 }
           else { if (xabs = yabs /\ xabs < 7 /\ xabs > 2) then { _result_ := 3 }
           else { if ((xabs = 2 /\ (yabs = 2 \/ yabs = 6)) \/
                          (xabs = 6 /\ (yabs = 2))) then { _result_ := 2  }
           else { if ((xabs = 0 /\ (yabs = 4)) \/ (xabs = 1 /\ (yabs = 1 \/ yabs = 5)) \/
                      (xabs = 4 /\ (yabs = 0 \/ yabs = 7)) \/
                      (xabs = 5 /\ (yabs = 1)) \/
                      (xabs = 7 /\ (yabs = 4))) then { _result_ := 1 }
           else { if (xabs <= 7 /\ yabs <= 7) then { _result_ := 0 }
           else { _result_ := -1 } } } } }"

        let squares = 
            [(0, singleLetterScore); 
             (1, doubleLetterScore); 
             (2, tripleLetterScore);
             (3, doubleWordScore);
             (4, tripleWordScore)] |>
            Map.ofList

        let standardBoardProg : boardProg = {
          prog = standardBoardSource;
          squares = squares;
          usedSquare = 0;
          center = (0, 0);
          isInfinite = false;
          ppSquare = "" // There will be a function here later but you never have to 
                        // reason about it
        }


        [<EntryPoint>]
        let main argv =

            printfn "Testing 7.1\n"

            run pif "if" |> printfn "%A"
            run pif "if-some random text" |> printfn "%A"
            run pif "ithen-some random text" |> printfn "%A"

            printfn "\n\nTesting 7.2\n"

            run pletter "abcd" |> printfn "%A"
            run pletter "1234" |> printfn "%A"
            run palphanumeric "abcd" |> printfn "%A"
            run palphanumeric "1234" |> printfn "%A"
            run palphanumeric "!@#$" |> printfn "%A"
            run whitespaceChar " hello" |> printfn "%A"
            run whitespaceChar "hello " |> printfn "%A"
            run spaces "            hello" |> printfn "%A"
            run spaces "hello " |> printfn "%A"
            run spaces1 "            hello" |> printfn "%A"
            run spaces1 "hello " |> printfn "%A"

            printfn "\n\nTesting 7.3\n"

            run (pif .>*>. pthen) "if    then" |> printfn "%A"
            run (pif .>*> pthen) "if    then" |> printfn "%A"
            run (pif >*>. pthen) "if    then" |> printfn "%A"
            run (pif .>*>. pthen) "ifthen" |> printfn "%A"
            run (pif .>*>. pthen) "if   then    " |> printfn "%A"
            run (pif .>*>. pthen) "    if   then" |> printfn "%A"
            run (pif .>*>. pthen .>*>. pelse) "if  then        else" |> printfn "%A"

            printfn "\n\nTesting 7.4\n"

            run (parenthesise pint32)  "(    5    )" |> printfn "%A"
            run (parenthesise pthen) "(    then )" |> printfn "%A"
            run (parenthesise pint32)  "(    x    )" |> printfn "%A"
            run (parenthesise pint32)  "(  5    x )" |> printfn "%A"

            printfn "\n\nTesting 7.5\n"

            run pid "x" |> printfn "%A"
            run pid "x1" |> printfn "%A"
            run pid "1x" |> printfn "%A"
            run pid "longVariableName" |> printfn "%A"

            printfn "\n\nTesting 7.6\n"

            run (unop (pchar '-') pint32) "-5"                |> printfn "%A"
            run (unop (pchar '-') pint32) "-     5"           |> printfn "%A"
            run (unop (pchar '-') pint32 |>> ( * ) (-1)) "-5" |> printfn "%A"
            run (unop (pchar '-') pint32) "-     x"           |> printfn "%A"

            printfn "\n\nTesting 7.7\n"

            run (binop (pchar '+') pint32 pint32) "5 +  7"  |> printfn "%A"
            run (binop (pchar '+') pint32 pid) "5+var"      |> printfn "%A"
            run (binop (pchar '+') pint32 pint32 |>>
                      (fun (a, b) -> a + b)) "5 +  7"       |> printfn "%A"

            printfn "\n\nTesting 7.8\n"

            run AexpParse "4"                      |> printfn "%A"
            run AexpParse "x2"                     |> printfn "%A"
            run AexpParse "5 + y * 3"              |> printfn "%A"
            run AexpParse "(5 - y) * -3"           |> printfn "%A"
            run AexpParse "pointValue (x % 4) / 0" |> printfn "%A"


            printfn "\n\nTesting 7.9\n"

            run CexpParse "'x'"                                    |> printfn "%A"
            run CexpParse "toLower (toUpper( 'x'))"                |> printfn "%A"
            run CexpParse "toLower (toUpper 'x')"                  |> printfn "%A"
            run CexpParse "intToChar (charToInt (' '))"            |> printfn "%A"
            run AexpParse "charToInt (charValue (pointValue (5)))" |> printfn "%A"


            printfn "\n\nTesting 7.10\n"

            run BexpParse "true"                        |> printfn "%A"
            run BexpParse "false"                       |> printfn "%A"
            run BexpParse "5 > 4 \/ 3 >= 7"             |> printfn "%A"
            run BexpParse "~false"                      |> printfn "%A"
            run BexpParse "(5 < 4 /\ 6 <= 3) \/ ~false" |> printfn "%A"
            run BexpParse "(5 < 4 \/ 6 <= 3) \/ ~true"  |> printfn "%A"

            printfn "\n\nTesting 7.11\n"

            run stmntParse "x := 5" |> printfn "%A"
            run stmntParse "declare myVar" |> printfn "%A"
            run stmntParse "declaremyVar" |> printfn "%A"
            run stmntParse "declare x; x := 5" |> printfn "%A"
            run stmntParse "declare x; x := 5  ;y:=7" |> printfn "%A"
            run stmntParse "if (x < y) then { x := 5 } else { y := 7 }" |> printfn "%A"
            run stmntParse "if (x < y) then { x := 5 }" |> printfn "%A"
            run stmntParse "while (true) do {x5 := 0} " |> printfn "%A"

            let factorial = 
                "declare arg;
                 arg := 10;
                 declare result;
                 if (arg >= 0) then {
                     declare acc;
                     acc := 1;
                     x := 0;
                     while (arg <> x) do {
                         x := x + 1;
                         acc := acc * x
                     };
                     result := acc
                 }"
    
            run stmntParse factorial |> printfn "%A"

            printfn "\n\nTesting 7.12\n"

            (parseSquareFun tripleWordScore |> Map.find 0) Eval.hello 0 0 |> printfn "%A"
            (parseSquareFun tripleWordScore |> Map.find 0) Eval.hello 4 0 |> printfn "%A"
            (parseSquareFun tripleWordScore |> Map.find 0) Eval.hello 0 42 |> printfn "%A"
            (parseSquareFun tripleWordScore |> Map.find 1) Eval.hello 0 0 |> printfn "%A"
            (parseSquareFun tripleWordScore |> Map.find 1) Eval.hello 4 0 |> printfn "%A"
            (parseSquareFun tripleWordScore |> Map.find 1) Eval.hello 0 42 |> printfn "%A"

            printfn "\n\nTesting 7.13\n"

            let bf = 
                parseBoardFun 
                    standardBoardSource
                    (Map.ofList [(0, "single letter score"); 
                                 (1, "double letter score");
                                 (2, "triple letter score");
                                 (3, "double word score");
                                 (4, "triple word score")])

            printfn "%A" (bf (0, 0))
            printfn "%A" (bf (0, 4))
            printfn "%A" (bf (2, 2))
            printfn "%A" (bf (3, 3))
            printfn "%A" (bf (0, 7))
            printfn "%A" (bf (0, 42))
    

            printfn "\n\nTesting 7.14\n"

            let evalSquare w pos acc m =
                let acc' = Map.find 0 m w pos acc
                m |>
                Map.tryFind 1 |>
                Option.map (fun f -> f w pos acc') |>
                Option.defaultValue acc'
    
            let board = parseBoardProg standardBoardProg
        
            for y in -10..10 do
                for x in -10..10 do
                    printf "%s " (board.squares (x, y) |> 
                                  Option.map (evalSquare Eval.hello 1 2 >> string) |> 
                                  Option.defaultValue "#")
                printfn ""


            0 // return an integer exit code
