module PinetreeShop.Domain.Baskets.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling

module Persistence = PinetreeCQRS.Persistence.SqlServer
module Basket = PinetreeShop.Domain.Baskets.BasketAggregate

module private Helpers = 
    let load = Persistence.Events.loadAggregateEvents<Basket.Event> 0
    let commit = Persistence.Events.commitEvents
    let dequeue = Persistence.Commands.dequeueCommands<Basket.Command>

let handler = Basket.makeBasketCommandHandler Helpers.load Helpers.commit
let processCommandQueue() = Seq.map handler <!> Helpers.dequeue()