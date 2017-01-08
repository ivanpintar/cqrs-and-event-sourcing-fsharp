// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System
open Chessie.ErrorHandling

module EventProcessor = PinetreeShop.Domain.OrderProcess.EventProcessor

[<EntryPoint>]
let main arg = 
    let mutable lastHandledEvent = 0

    let rec loop() = 
        let (commandList, lastEvent) = EventProcessor.processEvents lastHandledEvent
        lastHandledEvent <- lastEvent
        match commandList with
        | Ok(r, _) -> 
            r 
            |> Seq.iter (fun r' -> 
                         match r' with
                         | Ok(r'', _) -> Seq.iter (fun r''' -> printfn "%A" r''') r''
                         | Bad f -> printfn "%A" f) 
        | Bad f -> printfn "%A" f
        
        System.Threading.Thread.Sleep(300)
        if Console.KeyAvailable then 
            match Console.ReadKey().Key with
            | ConsoleKey.Escape -> 0
            | _ -> loop()
        else loop()
    loop()
