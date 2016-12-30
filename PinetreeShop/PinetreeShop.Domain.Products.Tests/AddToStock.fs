module PinetreeShop.Domain.Products.Tests.AddToStock

open PinetreeShop.Domain.Tests.TestBase
open PinetreeShop.Domain.Products.ProductAggregate
open PinetreeShop.Domain.Products.Tests.Base
open PinetreeCQRS.Infrastructure.Commands
open PinetreeCQRS.Infrastructure.Events
open Xunit
open FSharpx.Validation
open System

[<Fact>]
let ``When AddToStock ProductQuantityChanged``() = 
    let aggregateId = Guid.NewGuid()
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId None
    let payload = AddToStock(15)
    let command = createCommand aggregateId payload None None None

    let result = handleCommand [ initialEvent ] command

    let expected = ProductQuantityChanged(15) |> createExpectedEvent command
    match result with
    | Success s -> 
        let actual = Seq.head s |> createComparableEvent
        Assert.Equal(expected, actual)
    | Failure f -> fail f.reasons

[<Fact>]
let ``When AddToStock not created Fail``() = 
    let aggregateId = Guid.NewGuid()
    let payload = AddToStock(15)
    let command = createCommand aggregateId payload None None None

    let result = handleCommand [] command

    let expected = createExpectedFailure command ["Product must be created first"]
    match result with
    | Success s -> 
        failwith "Did not fail"
    | Failure f ->
        let actual = createComparableFailure f
        Assert.Equal(expected, actual)

[<Fact>]
let ``When AddToStock negative Fail``() = 
    let aggregateId = Guid.NewGuid()
    let initialEvent = ProductCreated("Test product", 15m) |> createInitialEvent aggregateId None
    let payload = AddToStock(-15)
    let command = createCommand aggregateId payload None None None
    
    let result = handleCommand [ initialEvent ] command

    let expected = createExpectedFailure command ["Quantity must be a positive number"]
    match result with
    | Success s -> 
        failwith "Did not fail"
    | Failure f ->
        let actual = createComparableFailure f
        Assert.Equal(expected, actual)
