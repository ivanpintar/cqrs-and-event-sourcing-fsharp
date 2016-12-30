module PinetreeShop.Domain.Products.Tests.Create

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open PinetreeCQRS.Infrastructure.Types
open Xunit
open FSharpx.Validation
open System

let aggregateId = Guid.NewGuid() |> AggregateId

[<Fact>]
let ``When Create ProductCreated``() = 
    let command = Create("Test product", 15m) |> createCommand aggregateId (Expected(0), None, None, None)
    let result = handleCommand [] command
    let expected = ProductCreated("Test product", 15m) |> createExpectedEvent command 1
    result |> checkSuccess expected

[<Fact>]
let ``When Create with negative price Failure``() = 
    let command = Create("Test product", -15m) |> createCommand aggregateId (Expected(0), None, None, None)
    let result = handleCommand [] command
    let expected = createExpectedFailure command [ "Price must be a positive number" ]
    result |> checkFailure expected

[<Fact>]
let ``When Create already created Failure``() = 
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
    let command = Create("Test product", 15m) |> createCommand aggregateId (Expected(1), None, None, None)
    let result = handleCommand [ initialEvent ] command
    let expected = createExpectedFailure command [ "Product already created" ]
    result |> checkFailure expected