module PinetreeShop.Domain.Products.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open System

module Commands = PinetreeCQRS.Infrastructure.Commands
module Persistence = PinetreeCQRS.Persistence.SqlServer
module Product = PinetreeShop.Domain.Products.ProductAggregate

module private Helpers = 
    let load = Persistence.Events.loadAggregateEvents<Product.Event> 0
    let commit = Persistence.Events.commitEvents   
    
    let dequeue = Persistence.Commands.dequeueCommands<Product.Command>

let handleCommand = 
    Product.makeProductCommandHandler Helpers.load Helpers.commit

let processCommandQueue() = Commands.processCommandQueue Helpers.dequeue handleCommand
