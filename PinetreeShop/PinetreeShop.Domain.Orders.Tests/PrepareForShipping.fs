module PinetreeShop.Domain.Orders.Tests.PrepareForShipping

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Orders.OrderAggregate
open PinetreeShop.Domain.Orders.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System
open System.Linq

let aggregateId = Guid.NewGuid() |> AggregateId
let basketId = Guid.NewGuid() |> BasketId

let orderLine = 
    { ProductId = Guid.NewGuid() |> ProductId
      ProductName = "Test"
      Price = 2m
      Quantity = 2 }

[<Theory>]
[<InlineData("Pending", true)>]
[<InlineData("Cancelled", false)>]
[<InlineData("ReadyForShipping", false)>]
[<InlineData("Shipped", false)>]
[<InlineData("Delivered", false)>]
[<InlineData("NotCreated", false)>]
let ``When PrepareForShipping`` state isSuccess = 
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
    
    let command = PrepareForShipping |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = List.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents
    
    let errors = 
        match state with
        | "NotCreated" -> [ ValidationError "Wrong Order state NotCreated" :> IError ; ValidationError "No order lines" :> IError ]
        | _ -> [ ValidationError (sprintf "Wrong Order state %s" state) :> IError ]
    

    let expectedEvent = createExpectedEvent command 1 OrderReadyForShipping
    let checkResult r = 
        match isSuccess with
        | true -> checkSuccess [ expectedEvent ] r
        | false -> checkFailure errors r
    
    handleCommand initialEvents' command |> checkResult

[<Fact>]
let ``When PrepareForShipping no orders fail`` () =
    let initialEvent = OrderCreated(basketId, ShippingAddress "a", []) |>createInitialEvent aggregateId 0
    PrepareForShipping
    |> createCommand aggregateId (Irrelevant, None, None, None)
    |> handleCommand [ initialEvent ]
    |> checkFailure [ ValidationError "No order lines" ]
