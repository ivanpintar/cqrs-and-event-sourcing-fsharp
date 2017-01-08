module PinetreeShop.Domain.OrderProcess.EventProcessor

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling

module Persistence = PinetreeCQRS.Persistence.SqlServer
module OrderPM = PinetreeShop.Domain.OrderProcess.OrderProcessManager
module Order = PinetreeShop.Domain.Orders.OrderAggregate
module Basket = PinetreeShop.Domain.Baskets.BasketAggregate
module Products = PinetreeShop.Domain.Baskets.BasketAggregate

module private Helpers = 
    let load = Persistence.Events.loadProcessEvents 0
    let commit = Persistence.Events.commitEvents
    let enqueue = Persistence.Commands.queueCommands 

let handler lastHandledEvent = OrderPM.makeOrderProcessHandler (Helpers.load lastHandledEvent) Helpers.enqueue

let processEvents<'TEvent when 'TEvent :> IEvent> lastHandledEvent = 
    let events = Persistence.Events.loadAllEvents lastHandledEvent
    match events with
    | Ok (evts, _) -> 
        let evtList = evts |> Seq.toList |> List.rev
        match evtList with
        | [] -> (ok Seq.empty, lastHandledEvent)
        | h::tail -> 
            let lastEvent = h.EventNumber
            let commands = Seq.map (handler lastHandledEvent) <!> events
            (commands, lastEvent)
    | _ -> (ok Seq.empty, lastHandledEvent)