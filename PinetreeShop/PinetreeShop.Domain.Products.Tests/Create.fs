module PinetreeShop.Domain.Products.Tests.Create

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
let ``When Create ProductCreated``() = 
    let command = Create("Test product", 15m) |> createCommand aggregateId (Expected(0), None, None, None)
    let expected = ProductCreated("Test product", 15m) |> createExpectedEvent command 1
    handleCommand [] command |> checkSuccess [ expected ]

[<Fact>]
let ``When Create with negative price Failure``() = 
    Create("Test product", -15m)
    |> createCommand aggregateId (Expected(0), None, None, None)
    |> handleCommand []
    |> checkFailure [ ValidationError "Price must be a positive number" ]

[<Fact>]
let ``When Create already created Failure``() = 
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
    Create("Test product", 15m)
    |> createCommand aggregateId (Expected(1), None, None, None)
    |> handleCommand [ initialEvent ]
    |> checkFailure [ ValidationError "Product already created" ]
