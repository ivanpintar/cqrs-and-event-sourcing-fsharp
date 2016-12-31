module PinetreeShop.Domain.Products.ReadModel

open PinetreeCQRS.Infrastructure.Types
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Persistence.SqlServer
open PinetreeShop.Domain.Products.ProductAggregate

module Writer = 
    module private Helpers = 
        let insertProduct event = true
        let updateQuantity event = true
        let updateReserved event = true
        let updateReservedAndQuantity event = true
        let loadEvents = loadTypeEvents<Event>
        let loadLastEvent() = 0
        
        let handler (event : EventEnvelope<Event>) = 
            match event.payload with
            | ProductCreated(name, price) -> insertProduct event
            | ProductQuantityChanged qty -> updateQuantity event
            | ProductReserved qty -> updateReserved event
            | ProductReservationCanceled qty -> updateReserved event
            | ProductPurchased qty -> updateReservedAndQuantity event
            | _ -> true
    
    let handleEvents() = 
        Helpers.loadLastEvent() |> readAndHandleTypeEvents<bool, Event> Helpers.loadEvents Helpers.handler

module Reader = 
    let getProducts() = failwith "Not implemented"
