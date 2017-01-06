module PinetreeShop.Domain.Orders.Tests.Cancel

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
let orderLine = {
    ProductId = Guid.NewGuid() |> ProductId
    ProductName = "Test"
    Price = 2m
    Quantity = 2
}

[<Theory>]
[<InlineData("Pending", true)>]
[<InlineData("Cancelled", false)>]
[<InlineData("ReadyForShipping", true)>]
[<InlineData("Shipped", true)>]
[<InlineData("Delivered", false)>]
[<InlineData("NotCreated", false)>]
let ``When Cancel`` state isSuccess = 
    let initialEvent1 = OrderCreated(basketId, ShippingAddress "a")
    let initialEvents = 
        match state with
        | "Pending" -> [ initialEvent1 ]
        | "Cancelled" -> [ initialEvent1; OrderCancelled ]
        | "ReadyForShipping" -> [ initialEvent1; OrderReadyForShipping ]
        | "Shipped" -> [ initialEvent1; OrderShipped ]
        | "Delivered" -> [ initialEvent1; OrderDelivered ]
        | _ -> []

    let command = Cancel |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = Seq.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents

    let error = sprintf "Wrong Order state %s" state

    let checkResult r =
        match isSuccess with
        | true -> checkSuccess (createExpectedEvent command 1 OrderCancelled) r
        | false -> checkFailure [ ValidationError error ] r

    handleCommand initialEvents' command |> checkResult

