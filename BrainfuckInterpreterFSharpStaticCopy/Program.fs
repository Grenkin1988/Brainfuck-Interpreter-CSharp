open System
open BrainfuckExamples

type ProgrammData = {
    Programm: char[]
    NextCommandIndex: int
    ByteCells: list<byte>
    Pointer: int
}

let contains x = 
    Seq.exists ((=) x)

let replaceAt list index value =
    list
    |> List.mapi (fun i x -> if i = index then value else x)

let cleanInputProgram (validChars: seq<char>) text =
    text 
    |> Seq.filter(fun c -> contains c validChars) 
    |> String.Concat
    |> Seq.toArray

let moveToNextCommand programmData =
    { programmData with NextCommandIndex = programmData.NextCommandIndex + 1 }

let executeMovePointerToRightCommand programmData =
    let newPointer = programmData.Pointer + 1
    if newPointer >= programmData.ByteCells.Length then
        { programmData with Pointer = newPointer; ByteCells = List.append programmData.ByteCells [byte 0]; }
    else
        { programmData with Pointer = newPointer; }

let executeMovePointerToLeftCommand programmData =
    match programmData.Pointer with
    | 0 -> programmData
    | _ -> { programmData with Pointer = programmData.Pointer - 1; }

let changeCellValueAtPoint programmData newValue =
    { programmData with ByteCells = replaceAt programmData.ByteCells programmData.Pointer newValue; }

let executeIncreaseCurrentCellCommand programmData =
    programmData.ByteCells
    |> List.item programmData.Pointer
    |> (fun x -> x + byte 1)
    |> changeCellValueAtPoint programmData

let executeDecreaseCurrentCellCommand programmData =
    programmData.ByteCells
    |> List.item programmData.Pointer
    |> (fun x -> x - byte 1)
    |> changeCellValueAtPoint programmData

let executePrintCurrentCellCommand programmData =
    programmData.ByteCells
    |> List.item programmData.Pointer
    |> char
    |> Console.Write
    programmData

let executeReadToCurrentCellCommand programmData =
    Console.ReadKey().KeyChar
    |> byte
    |> changeCellValueAtPoint programmData

let rec findCorespondingLoopEndIndex programmData index innerLoopsCount =
    let command =
        programmData.Programm 
        |> Seq.item index
    match command with
        | '[' -> findCorespondingLoopEndIndex programmData (index + 1) (innerLoopsCount + 1)
        | ']' -> 
            match innerLoopsCount with
            | 0 -> index
            | _ -> findCorespondingLoopEndIndex programmData (index + 1) (innerLoopsCount - 1)
        | _ -> findCorespondingLoopEndIndex programmData (index + 1) innerLoopsCount
 
let executeLoopStartCommand programmData =
    let data =
        programmData.ByteCells
        |> List.item programmData.Pointer
    let nextCommandIndex = 
        if data = byte 0 then
            findCorespondingLoopEndIndex programmData (programmData.NextCommandIndex + 1) 0
        else
            programmData.NextCommandIndex + 1
    { programmData with NextCommandIndex = nextCommandIndex }

let rec findCorespondingLoopStartIndex programmData index innerLoopsCount =
    let command =
        programmData.Programm 
        |> Seq.item index
    match command with
        | '[' -> 
            match innerLoopsCount with
            | 0 -> index
            | _ -> findCorespondingLoopStartIndex programmData (index - 1) (innerLoopsCount - 1)
        | ']' -> findCorespondingLoopStartIndex programmData (index - 1) (innerLoopsCount + 1)
        | _ -> findCorespondingLoopStartIndex programmData (index - 1) innerLoopsCount

let executeLoopEndCommand programmData =
    let data =
        programmData.ByteCells
        |> List.item programmData.Pointer
    let nextCommandIndex = 
        if data = byte 0 then
            programmData.NextCommandIndex + 1
        else
            findCorespondingLoopStartIndex programmData (programmData.NextCommandIndex - 1) 0
    { programmData with NextCommandIndex = nextCommandIndex }


let rec processNextCommand (programmData: ProgrammData) =
    let command = 
        programmData.Programm 
        |> Seq.item programmData.NextCommandIndex
    let data =
        match command with
        | '>' -> executeMovePointerToRightCommand programmData |> moveToNextCommand
        | '<' -> executeMovePointerToLeftCommand programmData |> moveToNextCommand
        | '+' -> executeIncreaseCurrentCellCommand programmData |> moveToNextCommand
        | '-' -> executeDecreaseCurrentCellCommand programmData |> moveToNextCommand
        | '.' -> executePrintCurrentCellCommand programmData |> moveToNextCommand
        | ',' -> executeReadToCurrentCellCommand programmData |> moveToNextCommand
        | '[' -> executeLoopStartCommand programmData
        | ']' -> executeLoopEndCommand programmData
        | _ -> failwith "AAAAAAAAA!!!!"
    if data.NextCommandIndex < data.Programm.Length then
        processNextCommand data
    else 
        ignore

let run programm =
    let cleanProgramm = 
        programm
        |> cleanInputProgram ['>'; '<'; '+'; '-'; '.'; ','; '['; ']']
    let programmData = { Programm = cleanProgramm; NextCommandIndex = 0; ByteCells = [ byte 0 ]; Pointer = 0; }
    processNextCommand programmData

[<EntryPoint>]
let main argv =
    run BrainfuckExamples.HelloWorld
    |> ignore
    Console.ReadKey()
    |> ignore
    0
