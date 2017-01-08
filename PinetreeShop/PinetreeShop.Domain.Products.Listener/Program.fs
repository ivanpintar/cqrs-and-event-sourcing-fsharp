// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
open System
open Chessie.ErrorHandling

module ReadModel = PinetreeShop.Domain.Products.ReadModel
module CommandHandler = PinetreeShop.Domain.Products.CommandHandler

[<EntryPoint>]
let main arg = 
    let rec loop() = 
        let eventList = CommandHandler.processCommandQueue()
        let res =
            match eventList with
            | Ok(r, _) -> 
                r 
                |> Seq.iter (fun r' -> 
                             match r' with
                             | Ok(r'', _) -> Seq.iter (fun r''' -> printfn "%A" r''') r''
                             | Bad f -> printfn "%A" f) 
            | Bad f -> printfn "%A" f

        let eventRes = ReadModel.Writer.handleEvents()
        let res2 =
            match eventRes with
            | Ok(r, _) -> 
                r 
                |> Seq.iter (fun r' -> 
                             match r' with
                             | Ok(r'', _) ->  printfn "%A" r''
                             | Bad f -> printfn "%A" f) 
            | Bad f -> printfn "%A" f

        System.Threading.Thread.Sleep(300)
        loop()
    loop()
