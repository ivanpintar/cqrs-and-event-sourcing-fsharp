// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System

module ReadModel = PinetreeShop.Domain.Products.ReadModel
module CommandHandler = PinetreeShop.Domain.Products.CommandHandler

[<EntryPoint>]
let main arg = 
    let rec loop() = 
        CommandHandler.processCommandQueue() |> ignore
        ReadModel.Writer.handleEvents |> ignore

        if Console.KeyAvailable then 
            match Console.ReadKey().Key with
            | ConsoleKey.Escape -> 0
            | _ -> loop()
        else loop()
    loop()
