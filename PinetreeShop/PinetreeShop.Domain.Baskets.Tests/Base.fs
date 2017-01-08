module PinetreeShop.Domain.Baskets.Tests.Base

open PinetreeShop.Domain.Tests.TestBase
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Types
open Chessie.ErrorHandling
open System

module Basket = PinetreeShop.Domain.Baskets.BasketAggregate

let handleCommand initialEvents = 
    let lastEventNumber = List.fold (fun acc e -> e.EventNumber) 0 initialEvents
    let load id = ok initialEvents
    let commit e = List.map (fun e' -> { e' with EventNumber = lastEventNumber + 1 }) e |> ok
    Basket.makeBasketCommandHandler load commit