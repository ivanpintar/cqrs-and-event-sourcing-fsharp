module PinetreeShop.Domain.Orders.Tests.Create

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

[<Fact>]
let ``When Create OrderCreated``() = 
    let command = 
        Create(basketId, ShippingAddress "Address", [ orderLine ]) 
        |> createCommand aggregateId (Expected(0), None, None, None)
    let expected = OrderCreated(basketId, ShippingAddress "Address", [ orderLine ]) |> createExpectedEvent command 1
    handleCommand [] command |> checkSuccess [ expected ]

[<Fact>]
let ``When Create without shippingAddress``() = 
    Create(basketId, ShippingAddress "", [ orderLine ])
    |> createCommand aggregateId (Expected(0), None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "Shipping Address cannot be empty" ]

[<Fact>]
let ``When Create created fail``() = 
    let initialEvent = OrderCreated(basketId, ShippingAddress "a", [ orderLine ]) |> createInitialEvent aggregateId 1
    Create(basketId, ShippingAddress "a", [ orderLine ])
    |> createCommand aggregateId (Irrelevant, None, None, None)
    |> handleCommand [ initialEvent ]
    |> checkFailure [ ValidationError "Wrong Order state Pending" ]

[<Fact>]
let `` When Create no order lines``() = 
    Create(basketId, ShippingAddress "a", [])
    |> createCommand aggregateId (Irrelevant, None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "No order lines" ]
