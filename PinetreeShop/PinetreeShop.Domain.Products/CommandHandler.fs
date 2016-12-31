module PinetreeShop.Domain.Products.CommandHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
module Commands = PinetreeCQRS.Infrastructure.Commands
module Persistence = PinetreeCQRS.Persistence.SqlServer
module Product = PinetreeShop.Domain.Products.ProductAggregate

module private Helpers = 
    let load = Persistence.loadAggregateEvents<Product.Event> 0
    let commit = Persistence.commitEvents
    let onFailure e = 
        e.reasons
        |> Seq.map (fun r -> r.ToString())
        |> String.concat "; " 
        |> failwith

    let dequeue = Persistence.dequeueCommands<Product.Command>

let handleCommand cmd = 
    let handler = Product.commandHandler Helpers.load Helpers.commit Helpers.onFailure
    handler cmd

let processCommandQueue () =
    Commands.processCommandQueue Helpers.dequeue handleCommand