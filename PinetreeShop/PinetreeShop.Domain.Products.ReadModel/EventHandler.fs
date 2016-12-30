module PinetreeShop.Domain.Products.ReadModel.EventHandler

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open PinetreeShop.Domain.Products.ProductAggregate

let productCreated event = event

let loadEvents number : List<EventEnvelope<Event>> = []
let handler event = 
    match event with
    | _ -> ""

let handleEvents = readAndHandleEvents loadEvents handler