module PinetreeShop.Domain.Orders.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling

module Persistence = PinetreeCQRS.Persistence.SqlServer
module Order = PinetreeShop.Domain.Orders.OrderAggregate

module private Helpers = 
    let load = Persistence.Events.loadAggregateEvents<Order.Event> 0
    let commit = Persistence.Events.commitEvents
    let dequeue = Persistence.Commands.dequeueCommands<Order.Command>

let handler = Order.makeProductCommandHandler Helpers.load Helpers.commit
let processCommandQueue() = Seq.map handler <!> Helpers.dequeue()