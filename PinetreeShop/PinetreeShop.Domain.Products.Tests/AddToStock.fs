module PinetreeShop.Domain.Products.Tests.AddToStock

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open System

let aggregateId = Guid.NewGuid() |> AggregateId

[<Fact>]
let ``When AddToStock ProductQuantityChanged``() = 
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
    let command = AddToStock(15) |> createCommand aggregateId (Expected(1), None, None, None)
    let expected = ProductQuantityChanged(15) |> createExpectedEvent command 2
    handleCommand [ initialEvent ] command |> checkSuccess expected

[<Fact>]
let ``When AddToStock not created Fail``() = 
    AddToStock(15)
    |> createCommand aggregateId (Expected(0), None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "Product must be created" ]

[<Fact>]
let ``When AddToStock negative Fail``() = 
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
    AddToStock(-15)
    |> createCommand aggregateId (Expected(1), None, None, None)
    |> handleCommand [ initialEvent ]
    |> checkFailure [ ValidationError "Quantity must be a positive number" ]
