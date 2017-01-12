module PinetreeShop.Domain.Products.Tests.RemoveFromStock

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System

let aggregateId = Guid.NewGuid() |> AggregateId

let initialEvents = 
    [ ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
      ProductQuantityChanged(15) |> createInitialEvent aggregateId 2 ]

[<Fact>]
let ``When RemoveFromStock ProductQuantityChanged``() = 
    let command = RemoveFromStock(5) |> createCommand aggregateId (Expected(2), None, None, None)
    let expected = ProductQuantityChanged(-5) |> createExpectedEvent command 3
    handleCommand initialEvents command |> checkSuccess [ expected ]

[<Fact>]
let ``When RemoveFromStock less than available ProductQuantityChanged``() = 
    let reserved = ProductReserved(8) |> createInitialEvent aggregateId 3
    let initialEvents' = initialEvents @ [ reserved ]
    let command = RemoveFromStock(5) |> createCommand aggregateId (Expected(3), None, None, None)
    let result = handleCommand initialEvents' command
    let expected = ProductQuantityChanged(-5) |> createExpectedEvent command 4
    result |> checkSuccess [ expected ]

[<Fact>]
let ``When RemoveFromStock not created Fail``() = 
    RemoveFromStock(15)
    |> createCommand aggregateId (Expected(0), None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "Product must be created"
                      ValidationError "Not enough available items" ]

[<Fact>]
let ``When RemoveFromStock not enough items Fail``() = 
    RemoveFromStock(16)
    |> createCommand aggregateId (Expected(2), None, None, None)
    |> handleCommand initialEvents
    |> checkFailure [ ValidationError "Not enough available items" ]

[<Fact>]
let ``When RemoveFromStock more than available items Fail``() = 
    let reserved = ProductReserved(8) |> createInitialEvent aggregateId 3
    let initialEvents' = initialEvents @ [ reserved ]
    RemoveFromStock(10)
    |> createCommand aggregateId (Expected(3), None, None, None)
    |> handleCommand initialEvents'
    |> checkFailure [ ValidationError "Not enough available items" ]

[<Fact>]
let ``When RemoveFromStock negative Fail``() = 
    RemoveFromStock(-5)
    |> createCommand aggregateId (Expected(2), None, None, None)
    |> handleCommand initialEvents
    |> checkFailure [ ValidationError "Quantity must be a positive number" ]
