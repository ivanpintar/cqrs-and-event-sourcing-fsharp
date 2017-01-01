module PinetreeShop.Domain.Products.Tests.Base

open PinetreeShop.Domain.Tests.TestBase
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Types
open Chessie.ErrorHandling

module Product = PinetreeShop.Domain.Products.ProductAggregate

let handleCommand initialEvents = 
    let lastEventNumber = Seq.fold (fun acc e -> e.EventNumber) 0 initialEvents
    let load id = ok initialEvents
    let commit e = Seq.map (fun e' -> { e' with EventNumber = lastEventNumber + 1 }) e |> ok
    Product.makeProductCommandHandler load commit