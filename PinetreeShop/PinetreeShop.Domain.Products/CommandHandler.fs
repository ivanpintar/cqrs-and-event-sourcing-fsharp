module PinetreeShop.Domain.Products.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System
open Chessie.ErrorHandling

module Persistence = PinetreeCQRS.Persistence.SqlServer
module Product = PinetreeShop.Domain.Products.ProductAggregate

module private Helpers = 
    let load = Persistence.Events.loadAggregateEvents Product.productCategory 0
    let commit = Persistence.Events.commitEvents Product.productCategory
    let dequeue () = Persistence.Commands.dequeueCommands Product.productQueueName

let handler = Product.makeProductCommandHandler Helpers.load Helpers.commit
let processCommandQueue() = Seq.map handler <!> Helpers.dequeue ()