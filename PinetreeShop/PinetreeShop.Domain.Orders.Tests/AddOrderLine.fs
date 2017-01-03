module PinetreeShop.Domain.Orders.Tests.AddOrderLine

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

[<Fact>]
let ``When AddOrderLine OrderLineAdded`` ()= 
    let initialEvent = OrderCreated(basketId, ShippingAddress "a") |> createInitialEvent aggregateId 1

    let command = AddOrderLine (orderLine) |> createCommand aggregateId (Irrelevant, None, None, None)
    let expected = OrderLineAdded(orderLine) |> createExpectedEvent command 2
    handleCommand [initialEvent] command |> checkSuccess expected 

[<Theory>]
[<InlineData("Pending", true)>]
[<InlineData("Cancelled", false)>]
[<InlineData("ReadyForShipping", false)>]
[<InlineData("Shipped", false)>]
[<InlineData("Delivered", false)>]
[<InlineData("Nothing", false)>]
let ``When AddOrderLine not pending fail`` state isSuccess = 
    let initialEvent1 = OrderCreated(basketId, ShippingAddress "a")
    let initialEvents = 
        match state with
        | "Pending" -> [ initialEvent1 ]
        | "Cancelled" -> [ initialEvent1; OrderCancelled ]
        | "ReadyForShipping" -> [ initialEvent1; OrderReadyForShipping ]
        | "Shipped" -> [ initialEvent1; OrderShipped ]
        | "Delivered" -> [ initialEvent1; OrderDelivered ]
        | _ -> []

    let command = AddOrderLine (orderLine) |> createCommand aggregateId (Irrelevant, None, None, None)
    let initialEvents' = Seq.map (fun e -> createInitialEvent aggregateId 0 e) initialEvents

    let error =
        match state with
        | "Nothing" -> "Order not created"
        | _ -> (sprintf "Wrong Order state %s" state)

    let checkResult r =
        match isSuccess with
        | true -> checkSuccess (createExpectedEvent command 1 (OrderLineAdded orderLine)) r
        | false -> checkFailure [ ValidationError error ] r

    handleCommand initialEvents' command |> checkResult

