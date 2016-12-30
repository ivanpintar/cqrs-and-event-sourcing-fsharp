module PinetreeShop.Domain.Products.Tests.AddToStock

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
let ``When AddToStock ProductQuantityChanged``() = 
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
    let command = AddToStock(15) |> createCommand aggregateId (Expected(1), None, None, None)
    let result = handleCommand [ initialEvent ] command
    let expected = ProductQuantityChanged(15) |> createExpectedEvent command 2
    result |> checkSuccess expected

[<Fact>]
let ``When AddToStock not created Fail``() = 
    let command = AddToStock(15) |> createCommand aggregateId (Expected(0), None, None, None)
    let result = handleCommand [] command
    let expected = createExpectedFailure command [ "Product must be created" ]
    result |> checkFailure expected

[<Fact>]
let ``When AddToStock negative Fail``() = 
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId 1
    let command = AddToStock(-15) |> createCommand aggregateId (Expected(1), None, None, None)
    let result = handleCommand [ initialEvent ] command
    let expected = createExpectedFailure command [ "Quantity must be a positive number" ]
    result |> checkFailure expected
