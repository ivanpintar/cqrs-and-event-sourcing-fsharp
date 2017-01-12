module PinetreeShop.Domain.Orders.Tests.Ship

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Orders.OrderAggregate
open PinetreeShop.Domain.Orders.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System

let aggregateId = Guid.NewGuid() |> AggregateId
let basketId = Guid.NewGuid() |> BasketId

let orderLine = 
    { ProductId = Guid.NewGuid() |> ProductId
      ProductName = "Test"
      Price = 2m
      Quantity = 2 }

[<Theory>]
[<InlineData("Pending", false)>]
[<InlineData("Cancelled", false)>]
[<InlineData("ReadyForShipping", true)>]
[<InlineData("Shipped", false)>]
[<InlineData("Delivered", false)>]
[<InlineData("NotCreated", false)>]
let ``When Ship`` state isSuccess = 
    let initialEvents1 = 
        [ OrderCreated(basketId, ShippingAddress "a", [ orderLine ])
          OrderLineProductReserved orderLine.ProductId ]
    
    let initialEvents = 
        match state with
        | "Pending" -> initialEvents1
        | "Cancelled" -> initialEvents1 @ [ OrderCancelled ]
        | "ReadyForShipping" -> initialEvents1 @ [ OrderReadyForShipping ]
        | "Shipped" -> initialEvents1 @ [ OrderShipped ]
        | "Delivered" -> initialEvents1 @ [ OrderDelivered ]
        | _ -> []
    
    let command = Ship |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = List.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents
    let error = sprintf "Wrong Order state %s" state
    
    let expectedEvent = createExpectedEvent command 1 OrderShipped
    let checkResult r = 
        match isSuccess with
        | true -> checkSuccess [ expectedEvent ] r
        | false -> checkFailure [ ValidationError error ] r
    handleCommand initialEvents' command |> checkResult
