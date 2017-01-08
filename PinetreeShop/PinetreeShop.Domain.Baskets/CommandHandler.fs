module PinetreeShop.Domain.Baskets.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling

module Persistence = PinetreeCQRS.Persistence.SqlServer
module Basket = PinetreeShop.Domain.Baskets.BasketAggregate

module private Helpers = 
    let load = Persistence.Events.loadAggregateEvents Basket.basketCategory 0
    let commit = Persistence.Events.commitEvents Basket.basketCategory
    let dequeue () = Persistence.Commands.dequeueCommands Basket.basketQueueName

let handler = Basket.makeBasketCommandHandler Helpers.load Helpers.commit
let processCommandQueue() = Seq.map handler <!> Helpers.dequeue ()