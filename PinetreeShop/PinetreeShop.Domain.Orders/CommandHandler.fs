module PinetreeShop.Domain.Orders.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling

module Persistence = PinetreeCQRS.Persistence.SqlServer
module Order = PinetreeShop.Domain.Orders.OrderAggregate

module private Helpers = 
    let load = Persistence.Events.loadAggregateEvents Order.orderCategory 0
    let commit = Persistence.Events.commitEvents Order.orderCategory
    let dequeue () = Persistence.Commands.dequeueCommands Order.orderQueueName

let handler = Order.makeOrderCommandHandler Helpers.load Helpers.commit
let processCommandQueue() = Seq.map handler <!> Helpers.dequeue ()